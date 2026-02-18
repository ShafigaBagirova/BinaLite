using Application.Dtos.PropertyAdDtos;

namespace Application.Abstracts.Services;

public interface IPropertyAdService
{
    Task<bool> CreatePropertyAdAsync(CreatePropertyAdRequest request, List<MediaUploadInput>? media, 
        CancellationToken ct=default);
    Task<List<GetAllPropertyAdResponse>> GetAllAsync(CancellationToken ct=default);
    Task<GetByIdPropertyAdResponse?> GetByIdAsync(int Id,CancellationToken ct=default);
    Task<bool> UpdatePropertyAdAsync(int id, UpdatePropertyAdRequest request, List<MediaUploadInput>? addMedia, int[]? removeMediaIds,
    CancellationToken ct = default);
    Task<bool> DeletePropertyAdAsync(int Id,CancellationToken ct=default);
    Task<List<GetAllPropertyAdResponse>> GetMyAsync( CancellationToken ct = default);

}
