using System.ComponentModel.DataAnnotations;
using RestLS.Auth.Models;

namespace RestLS.Data.Entities;

public class Notification : IUserOwnedResource
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Content { get; set; }
    public string? Link { get; set; }
    
    [Required]
    public string OwnerId { get; set; }
    public ClinicUser Owner { get; set; }
}