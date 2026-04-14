using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public class UrgenzeService : IUrgenzeService
{
    private readonly AppDbContext _db;

    public UrgenzeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UrgenzaDto>> GetAllAsync()
    {
        return await _db.Urgenze
            .OrderBy(x => x.Id)
            .Select(x => new UrgenzaDto
            {
                Id = x.Id,
                Descrizione = x.Descrizione
            })
            .ToListAsync();
    }
}