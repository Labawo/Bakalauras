using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data.Dtos.Appoitments;
using RestLS.Data.Dtos.Recomendation;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;


[ApiController]
[Route("api/")]
public class PatientDoctorController : ControllerBase
{
    private readonly ITherapiesRepository _therapiesRepository;
    private readonly IAppointmentsRepository _appointmentRepository;
    private readonly IRecomendationsRepository _recomendationsRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<ClinicUser> _userManager;

    public PatientDoctorController(UserManager<ClinicUser> userManager, IAppointmentsRepository appointmentRepository, ITherapiesRepository therapiesRepository, IRecomendationsRepository recomendationsRepository, IAuthorizationService authorizationService)
    {
        _appointmentRepository = appointmentRepository;
        _therapiesRepository = therapiesRepository;
        _recomendationsRepository = recomendationsRepository;
        _authorizationService = authorizationService;
        _userManager = userManager;
    }
    
    [HttpGet ("getMyAppointments/{appointmentId}/getMyRecommendations")]
    [Authorize(Roles = ClinicRoles.Patient)]
    public async Task<IEnumerable<RecomendationDto>> GetManyForPatient(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetAsync(appointmentId);
        if (appointment == null) return new List<RecomendationDto>();
        
        var recomendations = await _recomendationsRepository.GetManyForPatientAsync(appointment.ID, User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        return recomendations.Select(o => new RecomendationDto(o.ID, o.Description, o.RecomendationDate));
    }

    [HttpGet("getWeeklyAppointments")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<IEnumerable<AppointmentDto>> GetManyforDoctors()
    {
        var appointments = await _appointmentRepository.GetManyForDoctorAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        var appointmentDtos = new List<AppointmentDto>();
        
        foreach (var appointment in appointments)
        {
            var patient = await _userManager.FindByIdAsync(appointment.PatientId);
            var patientUsername = patient?.UserName ?? "Unknown"; // If user not found, set username to "Unknown"

            appointmentDtos.Add(new AppointmentDto(
                appointment.ID, 
                appointment.Time, 
                appointment.Price, 
                patientUsername, 
                appointment.DoctorName
            ));
        }
        
        return appointmentDtos;
    }
    
    [HttpGet("getMyAppointments")]
    [Authorize(Roles = ClinicRoles.Patient)]
    public async Task<IEnumerable<AppointmentForPatientDto>> GetManyforPatients()
    {
        var appointments = await _appointmentRepository.GetManyForPatientAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        return appointments.Select(o => new AppointmentForPatientDto(o.ID, o.Time, o.Price, o.DoctorName));
    }
    
    [HttpDelete("getWeeklyAppointments/{appoitmentId}")]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult> Remove(int appoitmentId)
    {
        var appoitment = await _appointmentRepository.GetAsync(appoitmentId);
        if (appoitment == null)
            return NotFound();

        await _appointmentRepository.RemoveAsync(appoitment);

        // 204
        return NoContent();
    }
}