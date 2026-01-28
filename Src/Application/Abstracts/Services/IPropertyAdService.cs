using Application.Dtos;

namespace Application.Abstracts.Services;

public interface IPropertyAdService
{
    Task CreatePropertyAdAsync(CreatePropertyAdRequest request,CancellationToken ct=default);
    Task<List<GetAllPropertyAdResponse>> GetAllAsync(CancellationToken ct=default);
    Task<GetByIdPropertyAdResponse> GetByIdAsync(int Id,CancellationToken ct=default);
    Task UpdatePropertyAdAsync(int id, UpdatePropertyAdRequest request,CancellationToken ct = default);
    Task DeletePropertyAdAsync(int Id,CancellationToken ct=default);
}
