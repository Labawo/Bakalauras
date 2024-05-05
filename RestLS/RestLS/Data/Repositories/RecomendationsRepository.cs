using Microsoft.EntityFrameworkCore;
using RestLS.Data.Entities;

namespace RestLS.Data.Repositories;

public interface IRecomendationsRepository
{
    Task<Recomendation?> GetAsync(int therapyId, int appointmentId, int recomendationId);
    Task<IReadOnlyList<Recomendation>> GetManyAsync(int therapyId, int appointmentId);
    Task<IReadOnlyList<Recomendation>> GetManyForPatientAsync(int appointmentId, string patientId);
    Task CreateAsync(Recomendation recomendation);
    Task UpdateAsync(Recomendation recomendation);
    Task RemoveAsync(Recomendation recomendation);
}

public class RecomendationsRepository : IRecomendationsRepository
{
    private readonly LS_DbContext _lsDbContext;

    public RecomendationsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<Recomendation?> GetAsync(int therapyId, int appointmentId, int recomendationId)
    {
        return await _lsDbContext.Recomendations.FirstOrDefaultAsync(o => o.ID == recomendationId && o.Appoint.ID == appointmentId && o.Appoint.Therapy.Id == therapyId);
    }

    public async Task<IReadOnlyList<Recomendation>> GetManyForPatientAsync(int appointmentId, string patientId)
    {
        return await _lsDbContext.Recomendations.Where(o => o.Appoint.ID == appointmentId && o.Appoint.PatientId == patientId).ToListAsync();
    }
    
    public async Task<IReadOnlyList<Recomendation>> GetManyAsync(int therapyId, int appointmentId)
    {
        return await _lsDbContext.Recomendations.Where(o => o.Appoint.Therapy.Id == therapyId && o.Appoint.ID == appointmentId).ToListAsync();
    }

    public async Task CreateAsync(Recomendation recomendation)
    {
        _lsDbContext.Recomendations.Add(recomendation);
        await _lsDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Recomendation recomendation)
    {
        _lsDbContext.Recomendations.Update(recomendation);
        await _lsDbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Recomendation recomendation)
    {
        _lsDbContext.Recomendations.Remove(recomendation);
        await _lsDbContext.SaveChangesAsync();
    }
    
}