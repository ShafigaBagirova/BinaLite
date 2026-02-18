using Application.Dtos.PropertyAdDtos;

namespace Application.Abstracts.Services;

public interface IPropertyAdService
{
    Task<bool> CreatePropertyAdAsync(string UserId,CreatePropertyAdRequest request, List<MediaUploadInput>? media, 
        CancellationToken ct=default);
    Task<List<GetAllPropertyAdResponse>> GetAllAsync(CancellationToken ct=default);
    Task<GetByIdPropertyAdResponse?> GetByIdAsync(int Id,CancellationToken ct=default);
    Task<bool> UpdatePropertyAdAsync(int id,string UserId, UpdatePropertyAdRequest request, List<MediaUploadInput>? addMedia, int[]? removeMediaIds,
    CancellationToken ct = default);
    Task<bool> DeletePropertyAdAsync(int Id, string UserId,CancellationToken ct=default);
    Task<List<GetAllPropertyAdResponse>> GetMyAsync(string UserId, CancellationToken ct = default);

}
