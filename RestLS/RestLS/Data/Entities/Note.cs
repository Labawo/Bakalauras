using System.ComponentModel.DataAnnotations;
using RestLS.Auth.Models;

namespace RestLS.Data.Entities;

public class Note : IUserOwnedResource
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    
    [Required]
    public string OwnerId { get; set; }
    public ClinicUser Owner { get; set; }
}