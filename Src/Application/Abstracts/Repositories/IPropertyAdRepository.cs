using Domain.Entities;

namespace Application.Abstracts.Repositories;

public interface IPropertyAdRepository:IRepository<PropertyAd,int>
{
    Task<List<PropertyAd>> GetAllWithMediaAsync(CancellationToken ct = default);
    Task<PropertyAd?> GetByIdWithMediaAsync(int id, CancellationToken ct = default);
}
