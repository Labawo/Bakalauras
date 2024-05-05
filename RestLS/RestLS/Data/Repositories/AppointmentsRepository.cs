using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;

public interface IAppointmentsRepository
{
    Task<Appointment?> GetAsync(int therapyId, int appointmentId);
    Task<Appointment?> GetAsync(int appointmentId);
    Task<IReadOnlyList<Appointment>> GetManyAsync(int therapyId);
    Task<IReadOnlyList<Appointment>> GetManyForPatientAsync(string patientId);
    Task<IReadOnlyList<Appointment>> GetManyForDoctorAsync(string ownerId);
    Task<IReadOnlyList<Appointment>> GetManyForDoctorWithoutFilterAsync(string ownerId);
    Task CreateAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task RemoveAsync(Appointment appointment);
    Task RemoveRangeAsync(string patientId);
}

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly LS_DbContext _lsDbContext;

    public AppointmentsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }
    
    public async Task<Appointment?> GetAsync(int therapyId, int appointmentId)
    {
        return await _lsDbContext.Appointments.FirstOrDefaultAsync(o => o.ID == appointmentId && o.Therapy.Id == therapyId);
    }
    
    public async Task<Appointment?> GetAsync(int appointmentId)
    {
        return await _lsDbContext.Appointments.FirstOrDefaultAsync(o => o.ID == appointmentId);
    }

    public async Task<IReadOnlyList<Appointment>> GetManyAsync(int therapyId)
    {
        return await _lsDbContext.Appointments.Where(o => o.Therapy.Id == therapyId).ToListAsync();
    }
    
    public async Task<IReadOnlyList<Appointment>> GetManyForPatientAsync(string patientId)
    {
        try
        {
            return await _lsDbContext.Appointments
                .Where(o => o.PatientId == patientId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes
            Console.WriteLine($"Error in GetManyForPatientAsync: {ex.Message}");
            throw; // Rethrow the exception or handle it appropriately
        }
    }
    
    public async Task<IReadOnlyList<Appointment>> GetManyForDoctorAsync(string ownerId)
    {
        return await _lsDbContext.Appointments.Where(o => o.Therapy.OwnerId == ownerId && o.PatientId != null).ToListAsync();
    }
    
    public async Task<IReadOnlyList<Appointment>> GetManyForDoctorWithoutFilterAsync(string ownerId)
    {
        return await _lsDbContext.Appointments.Where(o => o.Therapy.OwnerId == ownerId).ToListAsync();
    }
    
    public async Task CreateAsync(Appointment appointment)
    {
        _lsDbContext.Appointments.Add(appointment);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Appointment appointment)
    {
        _lsDbContext.Appointments.Update(appointment);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Appointment appointment)
    {
        _lsDbContext.Appointments.Remove(appointment);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveRangeAsync(string patientId)
    {
        var appointmentsToRemove = _lsDbContext.Appointments
            .Where(appointment => appointment.PatientId == patientId);

        _lsDbContext.Appointments.RemoveRange(appointmentsToRemove);
        await _lsDbContext.SaveChangesAsync();
    }
}