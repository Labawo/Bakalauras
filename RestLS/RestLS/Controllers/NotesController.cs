using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data;
using RestLS.Data.Dtos.Notes;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly INotesRepository _notesRepository;
    private readonly IAuthorizationService _authorizationService;

    public NotesController(INotesRepository notesRepository, IAuthorizationService authorizationService)
    {
        _notesRepository = notesRepository;
        _authorizationService = authorizationService;
    }
    
    [HttpGet(Name = "GetNotes")]
    public async Task<IEnumerable<NoteDto>> GetManyPaging([FromQuery] NoteSearchParameters searchParameters)
    {
        var notes = await _notesRepository.GetManyAsync(searchParameters, User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        var previousPageLink = notes.HasPrevious
            ? CreateNotesResourceUri(searchParameters,
                RecourceUriType.PreviousPage)
            : null;
        
        var nextPageLink = notes.HasNext
            ? CreateNotesResourceUri(searchParameters,
                RecourceUriType.NextPage)
            : null;

        var paginationMetaData = new
        {
            totalCount = notes.TotalCount,
            pageSize = notes.PageSize,
            currentPage = notes.CurrentPage,
            totalPages = notes.TotalPages,
            previousPageLink,
            nextPageLink
        };

        if (paginationMetaData != null && !Response.Headers.ContainsKey("Pagination"))
        {
            // Add pagination metadata to response header
            Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetaData));
        }

        return notes.Select(o => new NoteDto(o.Id, o.Name, o.Content, o.OwnerId));
    }
    
    [HttpGet("{noteId}", Name = "GetNote")]
    public async Task<ActionResult<NoteDto>> Get(int noteId)
    {
        var note = await _notesRepository.GetAsync(noteId);
        
        //404
        if (note == null)
        {
            return NotFound();
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, note, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var links = CreateLinksForNotes(noteId);

        var noteDto = new NoteDto(note.Id, note.Name, note.Content, note.OwnerId);
        
        return Ok(new { Resource = noteDto, Links = links});
    }
    
    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create(CreateNoteDto createNoteDto)
    {
        var note = new Note
        {
            Name = createNoteDto.Name,
            Content = createNoteDto.Content,
            Time = DateTime.UtcNow,
            OwnerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        };

        await _notesRepository.CreateAsync(note);

        //201
        return Created("", new NoteDto(note.Id, note.Name, note.Content, note.OwnerId));
    }


    [HttpPut]
    [Route("{noteId}")]
    public async Task<ActionResult<NoteDto>> Update(int noteId, UpdateNoteDto updateNoteDto)
    {
        var note = await _notesRepository.GetAsync(noteId);

        if (note == null)
        {
            return NotFound();
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, note, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        note.Content = updateNoteDto.Content;

        await _notesRepository.UpdateAsync(note);

        return Ok(new NoteDto(note.Id, note.Name, note.Content, note.OwnerId));
    }

    
    [HttpDelete("{noteId}", Name = "DeleteNote")]
    public async Task<ActionResult> Remove(int noteId)
    {
        var note = await _notesRepository.GetAsync(noteId);

        if (note == null)
        {
            return NotFound();
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, note, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        await _notesRepository.RemoveAsync(note);
        
        //204
        return NoContent();
    }

    private IEnumerable<LinkDto> CreateLinksForNotes(int noteId)
    {
        yield return new LinkDto{ Href = Url.Link("GetNote", new {noteId}), Rel = "self", Method = "GET"};
        yield return new LinkDto{ Href = Url.Link("DeleteNote", new {noteId}), Rel = "delete_topic", Method = "DELETE"};
    }

    private string? CreateNotesResourceUri(NoteSearchParameters noteSearchParameters, RecourceUriType type)
    {
        return type switch
        {
            RecourceUriType.PreviousPage => Url.Link("GetNotes",
                new
                {
                    pageNumber = noteSearchParameters.PageNumber - 1,
                    pageSize = noteSearchParameters.PageSize,
                }),
            RecourceUriType.NextPage => Url.Link("GetNotes",
                new
                {
                    pageNumber = noteSearchParameters.PageNumber + 1,
                    pageSize = noteSearchParameters.PageSize,
                }),
            _ => Url.Link("GetNotes",
                new
                {
                    pageNumber = noteSearchParameters.PageNumber,
                    pageSize = noteSearchParameters.PageSize,
                })
        };
    }
}