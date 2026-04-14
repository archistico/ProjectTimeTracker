using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public class StatiService : IStatiService
{
    private readonly AppDbContext _db;

    public StatiService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<StatoDto>> GetAllAsync()
    {
        return await _db.Stati
            .OrderBy(x => x.Id)
            .Select(x => new StatoDto
            {
                Id = x.Id,
                Descrizione = x.Descrizione
            })
            .ToListAsync();
    }
}