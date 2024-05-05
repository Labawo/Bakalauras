using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data.Dtos.Appoitments;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ClinicUser> _userManager;
    private readonly IAppointmentsRepository _appointmentRepository;
    
    public UsersController(UserManager<ClinicUser> userManager, IAppointmentsRepository appointmentRepository)
    {
        _userManager = userManager;
        _appointmentRepository = appointmentRepository;
    }
    
    [HttpGet]
    [Route("patients")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public IActionResult GetPatients()
    {
        if (User.IsInRole(ClinicRoles.Admin))
        {
            return Forbid();
        }
        
        //var doctorId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var appointments = _appointmentRepository.GetManyForDoctorAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        
        // Create a HashSet to store unique patient IDs from appointments
        var appointmentPatientIds = new HashSet<string>();
        foreach (var appointment in appointments.Result)
        {
            appointmentPatientIds.Add(appointment.PatientId); 
        }
        
        var patients = _userManager.GetUsersInRoleAsync(ClinicRoles.Patient)
            .Result
            .Where(u => !_userManager.IsInRoleAsync(u, ClinicRoles.Admin).Result) // Filter out Admin users
            .Select(u => new UserDto(u.Id, u.UserName, u.Email))
            .ToList();
        
        var patientsWithAppointments = patients.Where(p => appointmentPatientIds.Contains(p.Id)).ToList();

        return Ok(patientsWithAppointments);
    }
    
    [HttpGet]
    [Route("users")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public IActionResult GetUsers()
    {
        var allUsers =  _userManager.Users.ToList(); 

        var nonAdminUsers = allUsers
            .Where(u => !_userManager.IsInRoleAsync(u, ClinicRoles.Admin).Result) // Filter out Admin users
            .Select(u => new UserDto(u.Id, u.UserName, u.Email))
            .ToList();

        return Ok(nonAdminUsers);
    }

    [HttpGet]
    [Route("doctors")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public IActionResult GetDoctors()
    {
        var doctorUsers = _userManager.GetUsersInRoleAsync(ClinicRoles.Doctor)
            .Result
            .Where(u => !_userManager.IsInRoleAsync(u, ClinicRoles.Admin).Result) // Exclude users with the Admin role
            .Select(u => new UserDto(u.Id, u.UserName, u.Email))
            .ToList();

        return Ok(doctorUsers);
    }
    
    [HttpDelete]
    [Route("users/{userId}")]
    [Authorize(Roles = ClinicRoles.Admin)] // Assuming only administrators can delete users
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (User.IsInRole(ClinicRoles.Patient))
        {
            await _appointmentRepository.RemoveRangeAsync(userId);
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return NoContent(); // 204 No Content - Successful deletion
        }
        // If the deletion was not successful, return the errors
        return BadRequest(result.Errors);
    }
    
    [HttpPost]
    [Route("registerDoctor")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> RegisterDoctor(RegisterDoctorDto registerDoctorDto)
    {
        var user = await _userManager.FindByNameAsync(registerDoctorDto.UserName);

        if (user != null)
            return BadRequest("This user already exists.");

        var newUser = new ClinicUser
        {
            Email = registerDoctorDto.Email,
            UserName = registerDoctorDto.UserName,
            Name = registerDoctorDto.Name,
            LastName = registerDoctorDto.LastName
        };

        var createUserResult = await _userManager.CreateAsync(newUser, registerDoctorDto.Password);

        if (!createUserResult.Succeeded)
            return BadRequest("Could not create a doctor.");

        await _userManager.AddToRoleAsync(newUser, ClinicRoles.Doctor);

        return CreatedAtAction(nameof(RegisterDoctor), new UserDto(newUser.Id, newUser.UserName, newUser.Email));
    }
    
    [HttpPut]
    [Route("changePassword/{userId}")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, changePasswordDto.NewPassword);

        if (result.Succeeded)
        {
            return Ok("Password changed successfully.");
        }
        return BadRequest(result.Errors);
    }
    
    /*[HttpPut]
    [Route("validateUser/{userId}")]
    [Authorize(Roles = ClinicRoles.Admin)] // Assuming only administrators can update user parameters
    public async Task<IActionResult> ValidateUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Update user parameters
        user.IsVerified = true;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok("User parameters updated successfully.");
        }
        return BadRequest(result.Errors);
    }*/
}