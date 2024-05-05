namespace RestLS.Auth.Models;

public interface IUserOwnedResource
{
    public string OwnerId { get; }
}