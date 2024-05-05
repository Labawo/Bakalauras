namespace RestLS.Data.Dtos.Notifications;

public record NotificationDto(int Id, string Content, string? Link, string OwnerId);
public record CreateNotificationDto(string Content, string Name);