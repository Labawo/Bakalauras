using System.ComponentModel.DataAnnotations;
using RestLS.Auth.Models;
namespace RestLS.Data.Entities;

public class Test : IUserOwnedResource
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Name { get; set; }
    public int DepressionScore { get; set; }
    public int AnxietyScore { get; set; }
    public string DepressionResults { get; set; }
    public string AnxietyResults { get; set; }
    
    [Required]
    public string OwnerId { get; set; }
    public ClinicUser Owner { get; set; }
}