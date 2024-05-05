using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestLS.Auth.Models;
using RestLS.Controllers;
using RestLS.Data.Dtos.Appoitments;
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
    public class AppointmentsControllerTests
    {
        [Fact]
        public async Task GetMany_ReturnsAppointments()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new AppointmentController(userManager, appointmentsRepository, therapiesRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "ownerId", ImageData = null };


            var appointments = new List<Appointment>
            {
                new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(1), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null },
                new Appointment { ID = 2, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null }
            };

            Assert.Equal(2, appointments.Count());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Patient) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetManyAsync(therapy.Id)).Returns(appointments);

            // Act
            var result = await controller.GetMany(therapy.Id);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Get_ReturnsAppointment()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new AppointmentController(userManager, appointmentsRepository, therapiesRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "ownerId", ImageData = null };

            var appointment = new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(1), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetAsync(therapy.Id, appointment.ID)).Returns(Task.FromResult(appointment));

            // Act
            var result = await controller.Get(therapy.Id, appointment.ID);

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
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new AppointmentController(userManager, appointmentsRepository, therapiesRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "ownerId", ImageData = null };
      
            var appointment = new CreateAppointmentDto(20, "2024-02-27");

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
           {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
           }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);
            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetManyForDoctorWithoutFilterAsync("doctorId")).Returns(new List<Appointment>());            

            // Act
            var result = await controller.Create(therapy.Id, appointment);

            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            Assert.NotNull(actionResult);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedResult()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new AppointmentController(userManager, appointmentsRepository, therapiesRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "ownerId", ImageData = null };

            var appointmentId = 1;

            var appointment = new Appointment { ID = appointmentId, Time = DateTime.UtcNow.AddHours(1), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            var updatedappointment = new UpdateAppointmentDto("2024-02-27", 25);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
           {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
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
            A.CallTo(() => appointmentsRepository.GetManyForDoctorWithoutFilterAsync("doctorId")).Returns(new List<Appointment>());

            // Act
            var result = await controller.Update(therapy.Id, appointmentId, updatedappointment);

            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            Assert.NotNull(actionResult);
        }

        [Fact]
        public async Task Select_ReturnsSelectedResult()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new AppointmentController(userManager, appointmentsRepository, therapiesRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "ownerId", ImageData = null };

            var appointmentId = 1;

            var appointment = new Appointment { ID = appointmentId, Time = DateTime.UtcNow.AddHours(1), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            var updatedappointment = new UpdateAppointmentDto("2024-02-27", 25);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Patient) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            A.CallTo(() => therapiesRepository.GetAsync(therapy.Id)).Returns(Task.FromResult(therapy));
            A.CallTo(() => appointmentsRepository.GetAsync(therapy.Id, appointment.ID)).Returns(Task.FromResult(appointment));
            A.CallTo(() => appointmentsRepository.GetManyForPatientAsync("userId")).Returns(new List<Appointment>());

            // Act
            var result = await controller.Select(therapy.Id, appointmentId);

            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            Assert.NotNull(actionResult);
        }

        [Fact]
        public async Task Remove_ReturnsNoContentResult()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var notificationsRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new AppointmentController(userManager, appointmentsRepository, therapiesRepository, authorizationService, notificationsRepository);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "ownerId", ImageData = null };

            var appointmentId = 1;

            var appointment = new Appointment { ID = appointmentId, Time = DateTime.UtcNow.AddHours(1), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
           {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
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
            var result = await controller.Remove(therapy.Id, appointmentId);

            var actionResult = Assert.IsType<NoContentResult>(result);
        }
    }
}
