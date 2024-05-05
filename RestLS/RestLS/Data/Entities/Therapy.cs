using System.ComponentModel.DataAnnotations;
using RestLS.Auth.Models;

namespace RestLS.Data.Entities;

public class Therapy : IUserOwnedResource
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageData { get; set; }
    
    [Required]
    public required string OwnerId { get; set; }
    
    public ClinicUser Owner { get; set; }
    
}