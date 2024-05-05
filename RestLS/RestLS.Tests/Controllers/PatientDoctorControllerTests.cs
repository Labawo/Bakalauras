using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestLS.Auth.Models;
using RestLS.Controllers;
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
    public class PatientDoctorControllerTests
    {
        [Fact]
        public async Task GetMany_ReturnsRecommendationsForPatients()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new PatientDoctorController(userManager, appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

            var appointment = new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = "userId" };

            var recommendations = new List<Recomendation>
            {
                new Recomendation { ID = 1, Description = "desc 1", Appoint = appointment, RecomendationDate = DateTime.UtcNow.AddHours(2)},
                new Recomendation { ID = 2, Description = "desc 1", Appoint = appointment, RecomendationDate = DateTime.UtcNow.AddHours(3)}
            };

            Assert.Equal(2, recommendations.Count());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Patient) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
                        
            A.CallTo(() => appointmentsRepository.GetAsync(appointment.ID)).Returns(Task.FromResult(appointment));
            A.CallTo(() => recommendationsRepository.GetManyForPatientAsync(appointment.ID, null)).Returns(recommendations);

            // Act
            var result = await controller.GetManyForPatient(appointment.ID);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetMany_ReturnsAppointmentsForDoctors()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new PatientDoctorController(userManager, appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

            var appointments = new List<Appointment>
            {
                new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(1), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null },
                new Appointment { ID = 2, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = null, PatientId = null }
            };

            Assert.Equal(2, appointments.Count());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Doctor) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            A.CallTo(() => appointmentsRepository.GetManyForDoctorAsync(null)).Returns(appointments);

            // Act
            var result = await controller.GetManyforDoctors();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetMany_ReturnsAppointmentsForPatients()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new PatientDoctorController(userManager, appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

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

            A.CallTo(() => appointmentsRepository.GetManyForPatientAsync(null)).Returns(appointments);

            // Act
            var result = await controller.GetManyforPatients();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Remove_ReturnsNoContentResult()
        {
            // Arrange
            var therapiesRepository = A.Fake<ITherapiesRepository>();
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var recommendationsRepository = A.Fake<IRecomendationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();
            var userManager = A.Fake<UserManager<ClinicUser>>();

            var controller = new PatientDoctorController(userManager, appointmentsRepository, therapiesRepository, recommendationsRepository, authorizationService);

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "userId", ImageData = null };

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
                        
            A.CallTo(() => appointmentsRepository.GetAsync(appointment.ID)).Returns(Task.FromResult(appointment));

            // Act
            var result = await controller.Remove(appointmentId);

            var actionResult = Assert.IsType<NoContentResult>(result);
        }
    }
}
