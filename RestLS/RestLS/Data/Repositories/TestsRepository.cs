using Microsoft.EntityFrameworkCore;
using RestLS.Data.Dtos.Tests;
using RestLS.Data.Entities;
using RestLS.Helpers;

namespace RestLS.Data.Repositories;

public interface ITestsRepository
{
    Task<Test?> GetAsync(int testId);
    Task<Test?> GetLastTestForPatientAsync(string ownerId);
    Task<PagedList<Test>> GetManyAsync(TestSearchParameters testSearchParameters, string ownerId);
    Task CreateAsync(Test test);
    Task RemoveAsync(Test test);
}

public class TestsRepository : ITestsRepository
{
    private readonly LS_DbContext _lsDbContext;
    
    public TestsRepository(LS_DbContext lsDbContext)
    {
        _lsDbContext = lsDbContext;
    }

    public async Task<Test?> GetAsync(int testId)
    {
        return await _lsDbContext.Tests.FirstOrDefaultAsync(o => o.Id == testId);
    }
    
    public async Task<Test?> GetLastTestForPatientAsync(string ownerId)
    {
        return await _lsDbContext.Tests
            .Where(t => t.OwnerId == ownerId)
            .OrderByDescending(t => t.Time)
            .FirstOrDefaultAsync();
    }
    
    public async Task<PagedList <Test>> GetManyAsync(TestSearchParameters testSearchParameters, string ownerId)
    {
        var queryable = _lsDbContext.Tests.AsQueryable().Where(o => o.OwnerId == ownerId).OrderByDescending(o => o.Time);
        
        return await PagedList<Test>.CreateAsync(queryable, testSearchParameters.PageNumber, testSearchParameters.PageSize);
    }

    public async Task CreateAsync(Test test)
    {
        _lsDbContext.Tests.Add(test);
        await _lsDbContext.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(Test test)
    {
        _lsDbContext.Tests.Remove(test);
        await _lsDbContext.SaveChangesAsync();
    }
}