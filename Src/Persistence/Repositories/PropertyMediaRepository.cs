using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PropertyMediaRepository : GenericRepository<PropertyMedia, int>, IPropertyMediaRepository
{
    private readonly BinaLiteDbContext _context;
    public PropertyMediaRepository(BinaLiteDbContext context) : base(context)
    {
        _context = context;
    }
    public Task<List<PropertyMedia>> GetByPropertyAdIdAsync(int propertyAdId, CancellationToken ct = default)
    {
        return _context.Set<PropertyMedia>()
           .Where(x => x.PropertyAdId == propertyAdId)
           .OrderBy(x => x.Order)
           .ToListAsync(ct);
    }
}
