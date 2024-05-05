using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestLS.Auth.Models;
using RestLS.Controllers;
using RestLS.Data.Dtos.Tests;
using RestLS.Data.Dtos.Therapies;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;
using RestLS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestLS.Tests.Controllers
{
    public class TestsControllerTests
    {
        [Fact]
        public async Task GetManyPaging_ReturnsTests()
        {
            // Arrange
            var testsRepository = A.Fake<ITestsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new TestsController(testsRepository, authorizationService);
            var searchParameters = new TestSearchParameters
            {
                PageNumber = 1,
                PageSize = 2 // Or any other appropriate value
            };

            var tests = new List<Test>
            {
                new Test { Id = 1, Name = "Therapy 1", Score = 21, OwnerId = "owner1" },
                new Test { Id = 2, Name = "Therapy 2", Score = 22, OwnerId = "owner1" }
            };

            A.CallTo(() => testsRepository.GetManyAsync(searchParameters, null)).Returns(Task.FromResult(new PagedList<Test>(tests, 2, 1, 2)));

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Headers.Add("Pagination", "your_pagination_value");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.GetManyPaging(searchParameters);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ReturnsTest()
        {
            // Arrange
            var testsRepository = A.Fake<ITestsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new TestsController(testsRepository, authorizationService);
            var testId = 1;

            var test = new Test { Id = testId, Name = "Therapy 1", Score = 21, OwnerId = "owner1" };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => testsRepository.GetAsync(testId)).Returns(Task.FromResult(test));

            // Act
            var result = await controller.Get(testId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var value = ((OkObjectResult)result.Result).Value;
            Assert.NotNull(value);

            var properties = value.GetType().GetProperties();

            // Access Resource and Links properties using reflection
            var resourceProperty = properties.FirstOrDefault(p => p.Name == "Resource");
            var linksProperty = properties.FirstOrDefault(p => p.Name == "Links");

            // Assert that Resource and Links properties are not null
            Assert.NotNull(resourceProperty);
            Assert.NotNull(linksProperty);

            // Get values of Resource and Links properties
            var resource = resourceProperty.GetValue(value);
            var links = linksProperty.GetValue(value);

            // Assert that Resource and Links values are not null
            Assert.NotNull(resource);
            Assert.NotNull(links);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult()
        {
            // Arrange
            var testsRepository = A.Fake<ITestsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new TestsController(testsRepository, authorizationService);
            var createTestDto = new CreateTestDto
            (
                23
            );

            // Mock User property
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Patient) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.Create(createTestDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TestDto>>(result);
            Assert.NotNull(actionResult);

            // Additional assertions can be made if needed, such as checking the returned value
            var testDto = actionResult.GetType().GetProperties();
            var value = testDto.FirstOrDefault(p => p.Name == "Result");
            //Assert.Null(therapyDto);
            //Assert.Equal(createTherapyDto.Name, therapyDto.Name);
            //Assert.Equal(createTherapyDto.Description, therapyDto.Description);
        }
        
        [Fact]
        public async Task Remove_ReturnsNoContentResult()
        {
            // Arrange
            var testsRepository = A.Fake<ITestsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new TestsController(testsRepository, authorizationService);
            var testId = 1; 

            // Mock User property
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Doctor) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var test = new Test
            {
                Id = testId,
                Name = "Existing Therapy",
                Score = 20,
                Time = DateTime.UtcNow.AddHours(1),
                OwnerId = "ownerId" // Assuming ownerId for testing
            };

            A.CallTo(() => testsRepository.GetAsync(testId)).Returns(test);
            
            // Act
            var result = await controller.Remove(testId);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
        }
    }
}
