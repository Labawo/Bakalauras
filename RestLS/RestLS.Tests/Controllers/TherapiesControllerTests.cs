using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestLS.Controllers;
using RestLS.Data.Dtos.Therapies;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;
using RestLS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using RestLS.Auth.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace RestLS.Tests.Controllers
{
    public class TherapiesControllerTests
    {
        [Fact]
        public async Task GetManyPaging_ReturnsTherapies()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new TherapiesController(therapiesRepository, appointmentsRepository, authorizationService);
            var searchParameters = new TherapySearchParameters
            {
                PageNumber = 1,
                PageSize = 2 // Or any other appropriate value
            };

            var therapies = new List<Therapy>
            {
                new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "owner1", ImageData = null },
                new Therapy { Id = 2, Name = "Therapy 2", Description = "Description 2", OwnerId = "owner2", ImageData = null }
            };

            A.CallTo(() => therapiesRepository.GetManyAsync(searchParameters)).Returns(Task.FromResult(new PagedList<Therapy>(therapies, 2, 1, 2)));

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
        public async Task Get_ReturnsTherapy()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new TherapiesController(therapiesRepository, appointmentsRepository, authorizationService);
            var therapyId = 1;

            var therapy = new Therapy { Id = therapyId, Name = "Therapy 1", Description = "Description 1", OwnerId = "owner1", ImageData = null };

            A.CallTo(() => therapiesRepository.GetAsync(therapyId)).Returns(Task.FromResult(therapy));

            // Act
            var result = await controller.Get(therapyId);

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
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new TherapiesController(therapiesRepository, appointmentsRepository, authorizationService);
            var createTherapyDto = new CreateTherapyDto
            (   "New Therapy",
                "Description of new therapy",
                "doctor1",
                null
            );

            // Mock User property
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await controller.Create(createTherapyDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TherapyDto>>(result);
            Assert.NotNull(actionResult);

            // Additional assertions can be made if needed, such as checking the returned value
            var therapyDto = actionResult.GetType().GetProperties();
            var value = therapyDto.FirstOrDefault(p => p.Name == "Result");
            //Assert.Null(therapyDto);
            //Assert.Equal(createTherapyDto.Name, therapyDto.Name);
            //Assert.Equal(createTherapyDto.Description, therapyDto.Description);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedResult()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            var controller = new TherapiesController(therapiesRepository, appointmentsRepository, authorizationService);
            var updateTherapyDto = new UpdateTherapyDto
            (
                "Updated Therapy",
                "Updated description of therapy",
                "Updated image data"
            );
            var therapyId = 1; // Assuming therapyId for testing is 1

            // Mock User property
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var existingTherapy = new Therapy
            {
                Id = therapyId,
                Name = "Existing Therapy",
                Description = "Existing description of therapy",
                ImageData = "Existing image data",
                OwnerId = "ownerId" // Assuming ownerId for testing
            };

            A.CallTo(() => therapiesRepository.GetAsync(therapyId)).Returns(existingTherapy);
            

            // Act
            var result = await controller.Update(therapyId, updateTherapyDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TherapyDto>>(result);
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
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            var controller = new TherapiesController(therapiesRepository, appointmentsRepository, authorizationService);
            var therapyId = 1; // Assuming therapyId for testing is 1

            // Mock User property
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var existingTherapy = new Therapy
            {
                Id = therapyId,
                Name = "Existing Therapy",
                Description = "Existing description of therapy",
                ImageData = "Existing image data",
                OwnerId = "ownerId" // Assuming ownerId for testing
            };

            A.CallTo(() => therapiesRepository.GetAsync(therapyId)).Returns(existingTherapy);
            A.CallTo(() => appointmentsRepository.GetManyAsync(therapyId)).Returns(new List<Appointment>()); // Assuming no appointments exist

            // Act
            var result = await controller.Remove(therapyId);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
        }

    }
}
