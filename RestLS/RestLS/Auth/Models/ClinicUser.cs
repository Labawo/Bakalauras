using Microsoft.AspNetCore.Identity;

namespace RestLS.Auth.Models;

public class ClinicUser : IdentityUser
{
    [PersonalData]
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public DateTime? TestTimer { get; set; }
    public bool ForceRelogin { get; set; }
}