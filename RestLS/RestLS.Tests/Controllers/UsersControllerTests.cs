using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestLS.Auth;
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
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetMany_ReturnsPatients()
        {
            // Arrange
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var userManager = A.Fake<UserManager<ClinicUser>>();
            var notificationsRepository = A.Fake<INotificationsRepository>();

            var controller = new UsersController(userManager, appointmentsRepository, notificationsRepository);
                        
            var patients = new List<ClinicUser>
            {
                new ClinicUser { Id = "1", UserName = "user1", Email = "user1@email.com" },
                new ClinicUser { Id = "2", UserName = "user2", Email = "user2@email.com" }
            };

            var therapy = new Therapy { Id = 1, Name = "Therapy 1", Description = "Description 1", OwnerId = "ownerId", ImageData = null };

            var appointments = new List<Appointment>
            {
                new Appointment { ID = 1, Time = DateTime.UtcNow.AddHours(1), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = patients[0], PatientId = "1" },
                new Appointment { ID = 2, Time = DateTime.UtcNow.AddHours(3), Price = 20, DoctorName = "doc", Therapy = therapy, Patien = patients[1], PatientId = "2" }
            };

            Assert.Equal(2, patients.Count());
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

            A.CallTo(() => appointmentsRepository.GetManyForDoctorAsync(null)).Returns(Task.FromResult<IReadOnlyList<Appointment>>(appointments));
            A.CallTo(() => userManager.GetUsersInRoleAsync(ClinicRoles.Patient)).Returns(patients);

            // Act
            var result = controller.GetPatients() as OkObjectResult;
            var patientsWithAppointments = result.Value as List<UserDto>;

            Assert.NotNull(result);
            Assert.Equal(2, patientsWithAppointments.Count());
        }

        [Fact]
        public async Task GetMany_ReturnsDoctors()
        {
            // Arrange
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var userManager = A.Fake<UserManager<ClinicUser>>();
            var notificationsRepository = A.Fake<INotificationsRepository>();

            var controller = new UsersController(userManager, appointmentsRepository, notificationsRepository);

            var patients = new List<ClinicUser>
            {
                new ClinicUser { Id = "1", UserName = "user1", Email = "user1@email.com" },
                new ClinicUser { Id = "2", UserName = "user2", Email = "user2@email.com" }
            };

            Assert.Equal(2, patients.Count());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            A.CallTo(() => userManager.GetUsersInRoleAsync(ClinicRoles.Doctor)).Returns(patients);

            // Act
            var result = controller.GetDoctors() as OkObjectResult;
            var doctors = result.Value as List<UserDto>;

            Assert.NotNull(result);
            Assert.Equal(2, doctors.Count());
        }

        [Fact]
        public async Task GetMany_ReturnsUsers()
        {
            // Arrange
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var userManager = A.Fake<UserManager<ClinicUser>>();
            var notificationsRepository = A.Fake<INotificationsRepository>();

            var controller = new UsersController(userManager, appointmentsRepository, notificationsRepository);

            var patients = new List<ClinicUser>
            {
                new ClinicUser { Id = "1", UserName = "user1", Email = "user1@email.com" },
                new ClinicUser { Id = "2", UserName = "user2", Email = "user2@email.com" }
            };

            Assert.Equal(2, patients.Count());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            A.CallTo(() => userManager.Users).Returns(patients.AsQueryable());

            // Act
            var result = controller.GetUsers() as OkObjectResult;
            var doctors = result.Value as List<UserDto>;

            Assert.NotNull(result);
            Assert.Equal(2, doctors.Count());
        }

        [Fact]
        public async Task Remove_ReturnsNoContentResult()
        {
            // Arrange
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var userManager = A.Fake<UserManager<ClinicUser>>();
            var notificationsRepository = A.Fake<INotificationsRepository>();

            var controller = new UsersController(userManager, appointmentsRepository, notificationsRepository);

            var clinicUser = new ClinicUser { Id = "userId1", UserName = "user1", Email = "user1@email.com" };
                        
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
            }));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var deletionResult = IdentityResult.Success;
            A.CallTo(() => userManager.FindByIdAsync("userId1")).Returns(Task.FromResult(clinicUser));
            A.CallTo(() => appointmentsRepository.RemoveRangeAsync("userId1")).Returns(null);
            A.CallTo(() => userManager.DeleteAsync(clinicUser)).Returns(deletionResult);

            // Act
            var result = await controller.DeleteUser("userId1");

            var actionResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePassword_ReturnsOk()
        {
            // Arrange
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var userManager = A.Fake<UserManager<ClinicUser>>();
            var notificationsRepository = A.Fake<INotificationsRepository>();

            var controller = new UsersController(userManager, appointmentsRepository, notificationsRepository);

            var clinicUser = new ClinicUser { Id = "userId1", UserName = "user1", Email = "user1@email.com" };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"), // Simulate user ID
                new Claim(ClaimTypes.Role, ClinicRoles.Admin) // Simulate user role
            }));

            ChangePasswordDto changedPw = new ChangePasswordDto("newPass");

            string resetToken = "resetToken";

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var updatedResult = IdentityResult.Success;
            A.CallTo(() => userManager.FindByIdAsync("userId1")).Returns(Task.FromResult(clinicUser));
            A.CallTo(() => userManager.GeneratePasswordResetTokenAsync(clinicUser)).Returns(resetToken);
            A.CallTo(() => userManager.ResetPasswordAsync(clinicUser, resetToken, changedPw.NewPassword)).Returns(updatedResult);

            // Act
            var result = await controller.ChangePassword(clinicUser.Id, changedPw);

            var actionResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithValidModel_ReturnsCreatedAtAction()
        {
            // Arrange
            var appointmentsRepository = A.Fake<IAppointmentsRepository>();
            var userManager = A.Fake<UserManager<ClinicUser>>();
            var notificationsRepository = A.Fake<INotificationsRepository>();

            var controller = new UsersController(userManager, appointmentsRepository, notificationsRepository);

            ClinicUser nullUser = null;

            // Mock UserManager behavior
            A.CallTo(() => userManager.FindByNameAsync(A<string>._)).Returns(nullUser); // Assuming user does not exist
            A.CallTo(() => userManager.CreateAsync(A<ClinicUser>._, A<string>._)).Returns(IdentityResult.Success);

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
            var registerDoctorDto = new RegisterDoctorDto("testuser", "testuser@example.com", "Password123!", "Zilvinas", "Zvagulis");
            var result = await controller.RegisterDoctor(registerDoctorDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(UsersController.RegisterDoctor), createdAtActionResult.ActionName);
            Assert.Null(createdAtActionResult.ControllerName);
            Assert.Null(createdAtActionResult.RouteValues);
        }
    }
}
