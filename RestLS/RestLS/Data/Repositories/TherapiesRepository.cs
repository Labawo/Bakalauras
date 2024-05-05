using Microsoft.EntityFrameworkCore;
using RestLS.Data.Dtos.Therapies;
using RestLS.Data.Entities;
using RestLS.Helpers;

namespace RestLS.Data.Repositories;

public interface ITherapiesRepository
{
    Task<Therapy?> GetAsync(int therapyId);
    Task<IReadOnlyList<Therapy>> GetManyAsync();
    Task<PagedList<Therapy>> GetManyAsync(TherapySearchParameters therapySearchParameters);
    Task CreateAsync(Therapy therapy);
    Task UpdateAsync(Therapy therapy);
    Task RemoveAsync(Therapy therapy);
}

public class TherapiesRepository : ITherapiesRepository
{
    private readonly LS_DbContext _lsDbContext;
    
    public TherapiesRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<Therapy?> GetAsync(int therapyId)
    {
        return await _lsDbContext.Therapies.FirstOrDefaultAsync(o => o.Id == therapyId);
    }

    public async Task<IReadOnlyList<Therapy>> GetManyAsync()
    {
        return await _lsDbContext.Therapies.ToListAsync();
    }
    
    public async Task<PagedList <Therapy>> GetManyAsync(TherapySearchParameters therapySearchParameters)
    {
        var queryable = _lsDbContext.Therapies.AsQueryable().OrderBy(o => o.Name);
        
        return await PagedList<Therapy>.CreateAsync(queryable, therapySearchParameters.PageNumber, therapySearchParameters.PageSize);
    }

    public async Task CreateAsync(Therapy therapy)
    {
        _lsDbContext.Therapies.Add(therapy);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Therapy therapy)
    {
        _lsDbContext.Therapies.Update(therapy);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Therapy therapy)
    {
        _lsDbContext.Therapies.Remove(therapy);
        await _lsDbContext.SaveChangesAsync();
    }
}