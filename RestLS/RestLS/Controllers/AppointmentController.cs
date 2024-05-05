using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data.Dtos.Appoitments;
using RestLS.Data.Repositories;
using RestLS.Data.Entities;

namespace RestLS.Controllers;

[ApiController]
[Route("api/therapies/{therapyId}/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly UserManager<ClinicUser> _userManager;
    private readonly ITherapiesRepository _therapiesRepository;
    private readonly IAppointmentsRepository _appointmentRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly INotificationsRepository _notificationsRepository;

    public AppointmentController(UserManager<ClinicUser> userManager, IAppointmentsRepository appointmentRepository, ITherapiesRepository therapiesRepository, IAuthorizationService authorizationService, INotificationsRepository notificationsRepository)
    {
        _userManager = userManager;
        _appointmentRepository = appointmentRepository;
        _therapiesRepository = therapiesRepository;
        _authorizationService = authorizationService;
        _notificationsRepository = notificationsRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<AppointmentDto>> GetMany(int therapyId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return new List<AppointmentDto>();

        var appointments = await _appointmentRepository.GetManyAsync(therapy.Id);

        if (User.IsInRole(ClinicRoles.Patient))
        {
            // Filter appointments to show only available appointments for patients
            appointments = appointments.Where(appointment => appointment.PatientId == null && appointment.Time > DateTime.UtcNow).ToList();
        }

        return appointments.Select(o => 
            new AppointmentDto(
                o.ID, 
                o.Time, 
                o.Price, 
                o.PatientId, 
                o.DoctorName));
    }

    // /api/topics/1/posts/2
    [HttpGet("{appoitmentId}", Name = "GetAppointment")]
    public async Task<ActionResult<AppointmentDto>> Get(int therapyId, int appoitmentId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var appointment = await _appointmentRepository.GetAsync(therapy.Id, appoitmentId);
        if (appointment == null) return NotFound();

        return Ok(new AppointmentDto(appointment.ID, appointment.Time, appointment.Price, appointment.PatientId, appointment.DoctorName));
    }

    [HttpPost]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult<AppointmentDto>> Create(int therapyId, CreateAppointmentDto appoitmentDto)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        
        var appoitment = new Appointment{Price = appoitmentDto.Price};
        appoitment.Therapy = therapy;
        var userId = therapy.OwnerId;
        var doctor = await _userManager.FindByIdAsync(userId);

        if (doctor != null)
        {
            appoitment.DoctorName = doctor.UserName; // Replace FullName with the property name containing the doctor's name in your ApplicationUser model
        } 

        var existingAppointments = await _appointmentRepository.GetManyForDoctorWithoutFilterAsync(therapy.OwnerId);
        DateTime oneWeekFromNow = DateTime.UtcNow.AddDays(7);

        var weeklyAppointments = existingAppointments.Where(appointment =>
            appointment.Time >= DateTime.UtcNow && appointment.Time <= oneWeekFromNow
        );
        
        // Convert the appointment time from the DTO to DateTime
        DateTime newAppointmentStart = DateTime.Parse(appoitmentDto.Time);
        DateTime newAppointmentEnd = newAppointmentStart.AddHours(1);

        // Check if there's any existing appointment that overlaps with the new appointment
        if (existingAppointments.Any(appointment =>
                (appointment.Time >= newAppointmentStart && appointment.Time < newAppointmentEnd) ||
                (appointment.Time <= newAppointmentStart && appointment.Time.AddHours(1) > newAppointmentStart)
            ) || weeklyAppointments.Count() >= 12)
        {
            return Conflict("Appointment at this time already exists or you have reached appointment limit.");
        }

            
        appoitment.Time = DateTime.Parse(appoitmentDto.Time);

        if (appoitment.Time < DateTime.UtcNow)
        {
            return Forbid();
        }

        await _appointmentRepository.CreateAsync(appoitment);

        return Created("GetAppointment", new AppointmentDto(appoitment.ID, appoitment.Time, appoitment.Price, appoitment.PatientId, appoitment.DoctorName));
    }

    [HttpPut("{appoitmentId}")]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult<AppointmentDto>> Update(int therapyId, int appoitmentId, UpdateAppointmentDto updateappoitmentDto)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var oldAppoitment = await _appointmentRepository.GetAsync(therapyId, appoitmentId);
        if (oldAppoitment == null)
            return NotFound();

        //oldPost.Body = postDto.Body;
        oldAppoitment.Price = updateappoitmentDto.Price;
        
        if (oldAppoitment.Time < DateTime.UtcNow)
        {
            return Forbid();
        }

        if (oldAppoitment.Time != DateTime.Parse(updateappoitmentDto.Time))
        {
            var existingAppointments = await _appointmentRepository.GetManyForDoctorWithoutFilterAsync(therapy.OwnerId);
            DateTime oneWeekFromNow = DateTime.UtcNow.AddDays(7);

            var weeklyAppointments = existingAppointments.Where(appointment =>
                appointment.Time >= DateTime.UtcNow && appointment.Time <= oneWeekFromNow
            );
        
            // Convert the appointment time from the DTO to DateTime
            DateTime newAppointmentStart = DateTime.Parse(updateappoitmentDto.Time);
            DateTime newAppointmentEnd = newAppointmentStart.AddHours(1);

            // Check if there's any existing appointment that overlaps with the new appointment
            if (existingAppointments.Any(appointment =>
                    (appointment.Time >= newAppointmentStart && appointment.Time < newAppointmentEnd) ||
                    (appointment.Time <= newAppointmentStart && appointment.Time.AddHours(1) > newAppointmentStart)
                ) || weeklyAppointments.Count() >= 12)
            {
                return Conflict("Appointment at this time already exists or you have reached appointment limit.");
            }
            
            oldAppoitment.Time = DateTime.Parse(updateappoitmentDto.Time);
            
            if (oldAppoitment.Time < DateTime.UtcNow)
            {
                return Forbid();
            }
        }
        
        await _appointmentRepository.UpdateAsync(oldAppoitment);

        if (oldAppoitment.PatientId != null)
        {
            var notification = new Notification
            {
                Content = "Your selected appointment was changed.",
                Time = DateTime.UtcNow,
                OwnerId = oldAppoitment.PatientId
            };

            await _notificationsRepository.CreateAsync(notification);
        }

        return Ok(new AppointmentDto(oldAppoitment.ID, oldAppoitment.Time, oldAppoitment.Price, oldAppoitment.PatientId, oldAppoitment.DoctorName));
    }
    
    [HttpPut("{appoitmentId}/select")]
    [Authorize(Roles = ClinicRoles.Patient)]
    public async Task<ActionResult<AppointmentDto>> Select(int therapyId, int appoitmentId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");

        var oldAppoitment = await _appointmentRepository.GetAsync(therapyId, appoitmentId);
        if (oldAppoitment == null)
            return NotFound();

        string userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var existingAppointments = await _appointmentRepository.GetManyForPatientAsync(userId);
        DateTime oneWeekFromNow = DateTime.UtcNow.AddDays(7);

        var weeklyAppointments = existingAppointments.Where(appointment =>
            appointment.Time >= DateTime.UtcNow && appointment.Time <= oneWeekFromNow
        );
        if (existingAppointments.Any(appointment => appointment.Time >= oldAppoitment.Time && appointment.Time <= oldAppoitment.Time.AddHours(1)) || weeklyAppointments.Count() >= 4)
        {
            return Conflict("Appointment at this time already exists or you have reached appointment limit.");
        }

        if (oldAppoitment.PatientId == null)
        {
            oldAppoitment.PatientId = userId;
        }
        else
        {
            return Conflict("Appointment already has user.");
        }
        
        await _appointmentRepository.UpdateAsync(oldAppoitment);
        
        var notification = new Notification
        {
            Content = "You have new appointment at " + oldAppoitment.Time,
            Time = DateTime.UtcNow,
            OwnerId = therapy.OwnerId
        };

        await _notificationsRepository.CreateAsync(notification);

        return Ok(new AppointmentDto(oldAppoitment.ID, oldAppoitment.Time, oldAppoitment.Price, oldAppoitment.PatientId, oldAppoitment.DoctorName));
    }

    [HttpDelete("{appoitmentId}")]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult> Remove(int therapyId, int appoitmentId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);
        
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        
        var appoitment = await _appointmentRepository.GetAsync(therapyId, appoitmentId);
        if (appoitment == null)
            return NotFound();
        
        if (appoitment.PatientId != null)
        {
            var notification = new Notification
            {
                Content = "Appointment at " + appoitment.Time + " was removed.",
                Time = DateTime.UtcNow,
                OwnerId = appoitment.PatientId
            };
            
            await _notificationsRepository.CreateAsync(notification);
        }

        await _appointmentRepository.RemoveAsync(appoitment);
        
        // 204
        return NoContent();
    }
}