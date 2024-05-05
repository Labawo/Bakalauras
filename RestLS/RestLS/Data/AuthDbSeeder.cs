using Microsoft.AspNetCore.Identity;
using RestLS.Auth.Models;

namespace RestLS.Data;

public class AuthDbSeeder
{
    private readonly UserManager<ClinicUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthDbSeeder(UserManager<ClinicUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await AddDefaultRoles();
        await AddAdminUser();
    }

    private async Task AddAdminUser()
    {
        var newAdminUser = new ClinicUser
        {
            UserName = "admin",
            Email = "admin@admin.com"
        };

        var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
        
        if (existingAdminUser == null)
        {
            var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, password: "VeriSafePassword1!");

            if (createAdminUserResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(newAdminUser, ClinicRoles.All);
            }
        }
    }
    
    private async Task AddDefaultRoles()
    {
        foreach (var role in ClinicRoles.All)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}