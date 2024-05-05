namespace RestLS.Data.Dtos.Notifications;

public class NotificationSearchParameters
{
    private int _pageSize = 6;
    private const int MaxPageSize = 50;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}