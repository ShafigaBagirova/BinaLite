using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PropertyAdRepository : GenericRepository<PropertyAd, int>,IPropertyAdRepository
{
    private readonly BinaLiteDbContext _context ;
    public PropertyAdRepository(BinaLiteDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<PropertyAd>> GetAllWithMediaAsync(CancellationToken ct = default)
    {
        return await _context.PropertyAds
            .Include(x => x.MediaItems)
            .ToListAsync(ct);
    }
    public async Task<PropertyAd?> GetByIdWithMediaAsync(int id, CancellationToken ct = default)
    {
        return await _context.PropertyAds
            .Include(x => x.MediaItems)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
