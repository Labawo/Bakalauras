using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestLS.Auth.Models;
using RestLS.Controllers;
using RestLS.Data.Dtos.Appoitments;
using RestLS.Data.Dtos.Recomendation;
using RestLS.Data.Dtos.Therapies;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestLS.Tests.Controllers
{
    public class RecommendationsControllerTests
    {
        [Fact]
        public async Task GetMany_ReturnsRecommendations()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();           
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new RecomendationsController(appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

            var appointment = new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            var recommendations = new List<Recomendation>
            {
                new Recomendation { ID = 1, Description = "desc 1", Appoint = appointment, RecomendationDate = DateTime.UtcNow.AddHours(2)},
                new Recomendation { ID = 2, Description = "desc 1", Appoint = appointment, RecomendationDate = DateTime.UtcNow.AddHours(3)}
            };

            Assert.Equal(2, recommendations.Count());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Doctor) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetAsync(therapy.Id, appointment.ID)).Returns(Task.FromResult(appointment));
            A.CallTo(() => recommendationsRepository.GetManyAsync(therapy.Id, appointment.ID)).Returns(recommendations);

            // Act
            var result = await controller.GetMany(therapy.Id, appointment.ID);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ReturnsRecommendation()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new RecomendationsController(appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

            var appointment = new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            var recommendation = new Recomendation { ID = 1, Description = "desc 1", Appoint = appointment, RecomendationDate = DateTime.UtcNow.AddHours(2) };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Doctor) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetAsync(therapy.Id, appointment.ID)).Returns(Task.FromResult(appointment));
            A.CallTo(() => recommendationsRepository.GetAsync(therapy.Id, appointment.ID, recommendation.ID)).Returns(Task.FromResult(recommendation));

            // Act
            var result = await controller.Get(therapy.Id, appointment.ID, recommendation.ID);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var value = ((OkObjectResult)result.Result).Value;
            Assert.NotNull(value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new RecomendationsController(appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

            var appointment = new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            var createDto = new CreateRecomendationDto
            (   
                "Description 2"
            );
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Doctor) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetAsync(therapy.Id, appointment.ID)).Returns(Task.FromResult(appointment));

            // Act
            var result = await controller.Create(therapy.Id, appointment.ID, createDto);

            var actionResult = Assert.IsType<ActionResult<RecomendationDto>>(result);
            Assert.NotNull(actionResult);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedResult()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new RecomendationsController(appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

            var appointment = new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            var recommendation = new Recomendation { ID = 1, Description = "desc 1", Appoint = appointment, RecomendationDate = DateTime.UtcNow.AddHours(2) };

            var updateDto = new UpdateRecomendationDto
            (
                "Description 2"
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Doctor) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetAsync(therapy.Id, appointment.ID)).Returns(Task.FromResult(appointment));
            A.CallTo(() => recommendationsRepository.GetAsync(therapy.Id, appointment.ID, recommendation.ID)).Returns(Task.FromResult(recommendation));

            // Act
            var result = await controller.Update(therapy.Id, appointment.ID, recommendation.ID, updateDto);

            var actionResult = Assert.IsType<ActionResult<RecomendationDto>>(result);
            Assert.NotNull(actionResult);
        }

        [Fact]
        public async Task Remove_ReturnsNoContentResult()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new RecomendationsController(appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

            var appointment = new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            var recommendation = new Recomendation { ID = 1, Description = "desc 1", Appoint = appointment, RecomendationDate = DateTime.UtcNow.AddHours(2) };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Doctor) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetAsync(therapy.Id, appointment.ID)).Returns(Task.FromResult(appointment));
            A.CallTo(() => recommendationsRepository.GetAsync(therapy.Id, appointment.ID, recommendation.ID)).Returns(Task.FromResult(recommendation));

            // Act
            var result = await controller.Remove(therapy.Id, appointment.ID, recommendation.ID);

            var actionResult = Assert.IsType<NoContentResult>(result);
        }
    }
}
