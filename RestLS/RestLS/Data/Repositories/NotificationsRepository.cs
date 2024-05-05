using Microsoft.EntityFrameworkCore;
using RestLS.Data.Dtos.Notifications;
using RestLS.Data.Entities;
using RestLS.Helpers;

namespace RestLS.Data.Repositories;

public interface INotificationsRepository
{
    Task<Notification?> GetAsync(int notificationId);
    Task<PagedList<Notification>> GetManyAsync(NotificationSearchParameters notificationSearchParameters, string ownerId);
    Task CreateAsync(Notification notification);
    Task RemoveAsync(Notification notification);
}

public class NotificationsRepository : INotificationsRepository
{
    private readonly LS_DbContext _lsDbContext;
    
    public NotificationsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<Notification?> GetAsync(int notificationId)
    {
        return await _lsDbContext.Notifications.FirstOrDefaultAsync(o => o.Id == notificationId);
    }
    
    public async Task<PagedList <Notification>> GetManyAsync(NotificationSearchParameters notificationSearchParameters, string ownerId)
    {
        var queryable = _lsDbContext.Notifications.AsQueryable().Where(o => o.OwnerId == ownerId).OrderByDescending(o => o.Time);
        
        return await PagedList<Notification>.CreateAsync(queryable, notificationSearchParameters.PageNumber, notificationSearchParameters.PageSize);
    }

    public async Task CreateAsync(Notification notification)
    {
        _lsDbContext.Notifications.Add(notification);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Notification notification)
    {
        _lsDbContext.Notifications.Remove(notification);
        await _lsDbContext.SaveChangesAsync();
    }
}