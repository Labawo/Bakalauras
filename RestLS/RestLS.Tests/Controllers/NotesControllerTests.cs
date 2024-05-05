using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestLS.Auth.Models;
using RestLS.Controllers;
using RestLS.Data.Dtos.Notes;
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
    public class NotesControllerTests
    {
        [Fact]
        public async Task GetManyPaging_ReturnsNotes()
        {
            // Arrange
            var notesRepository = A.Fake<INotesRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new NotesController(notesRepository, authorizationService);
            var searchParameters = new NoteSearchParameters
            {
                PageNumber = 1,
                PageSize = 2 // Or any other appropriate value
            };

            var notes = new List<Note>
            {
                new Note { Id = 1, Name = "Note 1", Content = "Description 1", OwnerId = "owner1", Time = DateTime.UtcNow.AddHours(1) },
                new Note { Id = 2, Name = "Note 2", Content = "Description 2", OwnerId = "owner2", Time = DateTime.UtcNow.AddHours(2) }
            };

            A.CallTo(() => notesRepository.GetManyAsync(searchParameters, null)).Returns(Task.FromResult(new PagedList<Note>(notes, 2, 1, 2)));

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
        public async Task Get_ReturnsNote()
        {
            // Arrange
            var notesRepository = A.Fake<INotesRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new NotesController(notesRepository, authorizationService);
            var noteId = 1;

            var note = new Note { Id = noteId, Name = "Note 1", Content = "Description 1", OwnerId = "owner1", Time = DateTime.UtcNow.AddHours(1) };
            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => notesRepository.GetAsync(noteId)).Returns(Task.FromResult(note));

            // Act
            var result = await controller.Get(noteId);

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
            var notesRepository = A.Fake<INotesRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new NotesController(notesRepository, authorizationService);
            var createNoteDto = new CreateNoteDto
            (
                "Content of new note",
                "Note name"
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
            var result = await controller.Create(createNoteDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<NoteDto>>(result);
            Assert.NotNull(actionResult);

            // Additional assertions can be made if needed, such as checking the returned value
            var noteDto = actionResult.GetType().GetProperties();
            var value = noteDto.FirstOrDefault(p => p.Name == "Result");
            //Assert.Null(therapyDto);
            //Assert.Equal(createTherapyDto.Name, therapyDto.Name);
            //Assert.Equal(createTherapyDto.Description, therapyDto.Description);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedResult()
        {
            // Arrange
            var notesRepository = A.Fake<INotesRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new NotesController(notesRepository, authorizationService);

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            var updateNoteDto = new UpdateNoteDto
            (
                "Updated content of Note"
            );
            var noteId = 1;

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

            var existingNote = new Note { Id = noteId, Name = "Note 1", Content = "Description 1", OwnerId = "owner1", Time = DateTime.UtcNow.AddHours(1) };

            A.CallTo(() => notesRepository.GetAsync(noteId)).Returns(existingNote);


            // Act
            var result = await controller.Update(noteId, updateNoteDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<NoteDto>>(result);
            Assert.NotNull(actionResult);

            // Additional assertions can be made if needed, such as checking the returned value
            //var therapyDto = actionResult.Value;
            //Assert.NotNull(therapyDto);
            //Assert.Equal(updateTherapyDto.Name, therapyDto.Name);
            //Assert.Equal(updateTherapyDto.Description, therapyDto.Description);
        }

        [Fact]
        public async Task Remove_ReturnsNoContentResult()
        {
            // Arrange
            var notesRepository = A.Fake<INotesRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            var controller = new NotesController(notesRepository, authorizationService);
            var noteId = 1; 

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

            var existingNote = new Note { Id = noteId, Name = "Note 1", Content = "Description 1", OwnerId = "owner1", Time = DateTime.UtcNow.AddHours(1) };

            A.CallTo(() => notesRepository.GetAsync(noteId)).Returns(existingNote);

            // Act
            var result = await controller.Remove(noteId);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
        }
    }
}
