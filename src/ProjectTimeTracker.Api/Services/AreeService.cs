using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Dtos;

namespace ProjectTimeTracker.Api.Services;

public class AreeService : IAreeService
{
    private readonly AppDbContext _db;

    public AreeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<AreaDto>> GetAllAsync()
    {
        return await _db.Aree
            .OrderBy(x => x.Descrizione)
            .Select(x => new AreaDto
            {
                Id = x.Id,
                Descrizione = x.Descrizione
            })
            .ToListAsync();
    }
}