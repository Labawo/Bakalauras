using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data;
using RestLS.Data.Dtos.Notifications;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationsRepository _notificationsRepository;
    private readonly IAuthorizationService _authorizationService;

    public NotificationsController(INotificationsRepository notificationsRepository, IAuthorizationService authorizationService)
    {
        _notificationsRepository = notificationsRepository;
        _authorizationService = authorizationService;
    }
    
    [HttpGet(Name = "GetNotifications")]
    public async Task<IEnumerable<NotificationDto>> GetManyPaging([FromQuery] NotificationSearchParameters searchParameters)
    {
        var notifications = await _notificationsRepository.GetManyAsync(searchParameters, User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        var previousPageLink = notifications.HasPrevious
            ? CreateNotificationsResourceUri(searchParameters,
                RecourceUriType.PreviousPage)
            : null;
        
        var nextPageLink = notifications.HasNext
            ? CreateNotificationsResourceUri(searchParameters,
                RecourceUriType.NextPage)
            : null;

        var paginationMetaData = new
        {
            totalCount = notifications.TotalCount,
            pageSize = notifications.PageSize,
            currentPage = notifications.CurrentPage,
            totalPages = notifications.TotalPages,
            previousPageLink,
            nextPageLink
        };

        if (paginationMetaData != null && !Response.Headers.ContainsKey("Pagination"))
        {
            // Add pagination metadata to response header
            Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetaData));
        }

        return notifications.Select(o => new NotificationDto(o.Id, o.Content, o.Link, o.OwnerId));
    }
    
    [HttpGet("{notificationId}", Name = "GetNotification")]
    public async Task<ActionResult<NotificationDto>> Get(int notificationId)
    {
        var notification = await _notificationsRepository.GetAsync(notificationId);
        
        //404
        if (notification == null)
        {
            return NotFound();
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, notification, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var links = CreateLinksForNotifications(notificationId);

        var notificationDto = new NotificationDto(notification.Id, notification.Content, notification.Link, notification.OwnerId);
        
        return Ok(new { Resource = notificationDto, Links = links});
    }
    
    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create()
    {
        var notification = new Notification
        {
            Content = "hello",
            Time = DateTime.UtcNow,
            Link = null,
            //OwnerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        };

        await _notificationsRepository.CreateAsync(notification);

        //201
        return Created("", new NotificationDto(notification.Id, notification.Content, notification.Link, notification.OwnerId));
    }

    
    [HttpDelete("{notificationId}", Name = "DeleteNotification")]
    public async Task<ActionResult> Remove(int notificationId)
    {
        var notification = await _notificationsRepository.GetAsync(notificationId);

        if (notification == null)
        {
            return NotFound();
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, notification, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        await _notificationsRepository.RemoveAsync(notification);
        
        //204
        return NoContent();
    }

    private IEnumerable<LinkDto> CreateLinksForNotifications(int notificationId)
    {
        yield return new LinkDto{ Href = Url.Link("GetNotification", new {notificationId}), Rel = "self", Method = "GET"};
        yield return new LinkDto{ Href = Url.Link("DeleteNotification", new {notificationId}), Rel = "delete_topic", Method = "DELETE"};
    }

    private string? CreateNotificationsResourceUri(NotificationSearchParameters notificationSearchParameters, RecourceUriType type)
    {
        return type switch
        {
            RecourceUriType.PreviousPage => Url.Link("GetNotifications",
                new
                {
                    pageNumber = notificationSearchParameters.PageNumber - 1,
                    pageSize = notificationSearchParameters.PageSize,
                }),
            RecourceUriType.NextPage => Url.Link("GetNotifications",
                new
                {
                    pageNumber = notificationSearchParameters.PageNumber + 1,
                    pageSize = notificationSearchParameters.PageSize,
                }),
            _ => Url.Link("GetNotifications",
                new
                {
                    pageNumber = notificationSearchParameters.PageNumber,
                    pageSize = notificationSearchParameters.PageSize,
                })
        };
    }
}