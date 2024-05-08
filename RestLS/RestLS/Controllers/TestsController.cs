using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data;
using RestLS.Data.Dtos.Tests;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;
using RestLS.Helpers;

namespace RestLS.Controllers;

[ApiController]
[Route("api/tests")]
public class TestsController : ControllerBase
{
    private readonly ITestsRepository _testsRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<ClinicUser> _userManager;

    public TestsController(ITestsRepository testsRepository, IAuthorizationService authorizationService, UserManager<ClinicUser> userManager)
    {
        _testsRepository = testsRepository;
        _authorizationService = authorizationService;
        _userManager = userManager;
    }
    
    [HttpGet(Name = "GetTests")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<IEnumerable<TestDto>> GetManyPaging([FromQuery] TestSearchParameters searchParameters, string patientId = null)
    {
        // If patientId is not provided in the query parameters, use patientId associated with the user
        if (string.IsNullOrEmpty(patientId) || User.IsInRole(ClinicRoles.Admin))
        {
            return new List<TestDto>();
        }

        var tests = await _testsRepository.GetManyAsync(searchParameters, patientId);

        var previousPageLink = tests.HasPrevious
            ? CreateTestsResourceUri(searchParameters, RecourceUriType.PreviousPage)
            : null;
    
        var nextPageLink = tests.HasNext
            ? CreateTestsResourceUri(searchParameters, RecourceUriType.NextPage)
            : null;

        var paginationMetaData = new
        {
            totalCount = tests.TotalCount,
            pageSize = tests.PageSize,
            currentPage = tests.CurrentPage,
            totalPages = tests.TotalPages,
            previousPageLink,
            nextPageLink
        };

        if (paginationMetaData != null && !Response.Headers.ContainsKey("Pagination"))
        {
            // Add pagination metadata to response header
            Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetaData));
        }

        return tests.Select(o => new TestDto(o.Id, o.Name, o.DepressionScore, o.AnxietyScore, o.Time, o.OwnerId));
    }

    
    [HttpGet("{testId}", Name = "GetTest")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<ActionResult<TestDto>> Get(int testId)
    {
        var test = await _testsRepository.GetAsync(testId);
        
        //404
        if (test == null)
        {
            return NotFound();
        }
        
        if (User.IsInRole(ClinicRoles.Admin))
        {
            return Forbid();
        }

        var links = CreateLinksForTests(testId);

        var testDto = new TestDto(test.Id, test.Name, test.DepressionScore, test.AnxietyScore, test.Time, test.OwnerId);
        
        return Ok(new { Resource = testDto, Links = links});
    }
    
    [HttpPost]
    public async Task<ActionResult<TestDto>> Create(CreateTestDto createTestDto)
    {
        DateTime currentTimeUtc = DateTime.UtcNow;
        
        var test = new Test
        {
            Name = currentTimeUtc.ToString("yyyy-MM-dd HH:mm:ss") + " Test",
            OwnerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
            Time = currentTimeUtc
        };

        var finalResults = TestScoreCounter.GetFinalResult(createTestDto.Score);

        var depressionScore = finalResults[0].Score;
        var anxietyScore = finalResults[1].Score;
        var depressionResults = finalResults[0].Result;
        var anxietyResults = finalResults[1].Result;

        if (depressionScore == -1 || anxietyScore == -1 || depressionResults == null || anxietyResults == null)
        {
            return BadRequest();
        }

        test.DepressionScore = depressionScore;
        test.AnxietyScore = anxietyScore;
        test.DepressionResults = depressionResults;
        test.AnxietyResults = anxietyResults;
        
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (user.TestTimer < currentTimeUtc.AddMinutes(30))
        {
            return BadRequest();
        }

        user.TestTimer = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            await _testsRepository.CreateAsync(test);
        }
        else
        {
            return BadRequest("Failed to update user timer");
        }
      
        //201
        return Created("", new TestDto(test.Id, test.Name, test.DepressionScore, test.AnxietyScore, test.Time, test.OwnerId));
    }

    
    [HttpDelete("{testId}", Name = "DeleteTest")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<ActionResult> Remove(int testId)
    {
        var test = await _testsRepository.GetAsync(testId);

        if (test == null)
        {
            return NotFound();
        }

        if (User.IsInRole(ClinicRoles.Admin))
        {
            return Forbid();
        }

        await _testsRepository.RemoveAsync(test);
        
        //204
        return NoContent();
    }

    private IEnumerable<LinkDto> CreateLinksForTests(int testId)
    {
        yield return new LinkDto{ Href = Url.Link("GetTest", new {testId}), Rel = "self", Method = "GET"};
        yield return new LinkDto{ Href = Url.Link("DeleteTest", new {testId}), Rel = "delete_topic", Method = "DELETE"};
    }

    private string? CreateTestsResourceUri(TestSearchParameters testSearchParameters, RecourceUriType type)
    {
        return type switch
        {
            RecourceUriType.PreviousPage => Url.Link("GetTests",
                new
                {
                    pageNumber = testSearchParameters.PageNumber - 1,
                    pageSize = testSearchParameters.PageSize,
                }),
            RecourceUriType.NextPage => Url.Link("GetTests",
                new
                {
                    pageNumber = testSearchParameters.PageNumber + 1,
                    pageSize = testSearchParameters.PageSize,
                }),
            _ => Url.Link("GetTests",
                new
                {
                    pageNumber = testSearchParameters.PageNumber,
                    pageSize = testSearchParameters.PageSize,
                })
        };
    }
}