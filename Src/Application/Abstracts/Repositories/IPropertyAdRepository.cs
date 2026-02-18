using Domain.Entities;

namespace Application.Abstracts.Repositories;

public interface IPropertyAdRepository:IRepository<PropertyAd,int>
{
    Task<List<PropertyAd>> GetAllWithMediaAsync(CancellationToken ct = default);
    Task<PropertyAd?> GetByIdWithMediaAsync(int id, CancellationToken ct = default);
    Task<PropertyAd?> GetByIdWithOwnerAsync(int id, CancellationToken ct = default);
    Task<List<PropertyAd>> GetByOwnerAsync(string UserId, CancellationToken ct = default);
    Task<List<PropertyAd>> GetPendingAsync(CancellationToken ct = default);
    Task<List<PropertyAd>> GetByOwnerWithMediaAsync(string UserId, CancellationToken ct = default);
}
