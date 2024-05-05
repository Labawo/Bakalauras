using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestLS.Auth.Models;
using RestLS.Controllers;
using RestLS.Data.Dtos.Notifications;
using RestLS.Data.Dtos.Tests;
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
    public class NotificationsControllerTests
    {
        [Fact]
        public async Task GetManyPaging_ReturnsNotifications()
        {
            // Arrange
            var notificationRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new NotificationsController(notificationRepository, authorizationService);
            var searchParameters = new NotificationSearchParameters
            {
                PageNumber = 1,
                PageSize = 2 // Or any other appropriate value
            };

            var notifications = new List<Notification>
            {
                new Notification { Id = 1, Content = "Notification 1", Time = DateTime.UtcNow.AddHours(1), OwnerId = "owner1" },
                new Notification { Id = 2, Content = "Notification 1", Time = DateTime.UtcNow.AddHours(1), OwnerId = "owner1" }
            };

            A.CallTo(() => notificationRepository.GetManyAsync(searchParameters, null)).Returns(Task.FromResult(new PagedList<Notification>(notifications, 2, 1, 2)));

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
        public async Task Get_ReturnsNotification()
        {
            // Arrange
            var notificationRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new NotificationsController(notificationRepository, authorizationService);
            var notificationId = 1;

            var notification = new Notification { Id = notificationId, Content = "Notification 1", Time = DateTime.UtcNow.AddHours(1), OwnerId = "owner1" };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => notificationRepository.GetAsync(notificationId)).Returns(Task.FromResult(notification));

            // Act
            var result = await controller.Get(notificationId);

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
            var notificationRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new NotificationsController(notificationRepository, authorizationService);

            // Act
            var result = await controller.Create();

            // Assert
            var actionResult = Assert.IsType<ActionResult<NotificationDto>>(result);
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
            var notificationRepository = A.Fake<INotificationsRepository>();
            var authorizationService = A.Fake<IAuthorizationService>();

            var controller = new NotificationsController(notificationRepository, authorizationService);
            var notificationId = 1;

            var notification = new Notification { Id = notificationId, Content = "Notification 1", Time = DateTime.UtcNow.AddHours(1), OwnerId = "owner1" };

            var authorizationResult = AuthorizationResult.Success(); // Simulate successful authorization
            A.CallTo(() => authorizationService.AuthorizeAsync(A<ClaimsPrincipal>._, A<object>._, A<string>._))
                .Returns(authorizationResult);

            A.CallTo(() => notificationRepository.GetAsync(notificationId)).Returns(notification);

            // Act
            var result = await controller.Remove(notificationId);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
        }
    }
}
