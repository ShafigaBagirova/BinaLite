using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Dtos.PropertyAdDtos;
using Application.Validations.PropertyAdValidation;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Persistence.Repositories;
using System.Linq;

namespace Persistence.Services;

public class PropertyAdService : IPropertyAdService
{
    private readonly IPropertyAdRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePropertyAdRequest> _createValidator;
    private readonly IValidator<UpdatePropertyAdRequest> _updateValidator;
    private readonly IFileStorageService _fileStorage;
    private readonly IPropertyMediaRepository _mediaRepository;

    public PropertyAdService(
     IPropertyAdRepository repository,
     IPropertyMediaRepository mediaRepository,
     IFileStorageService fileStorage,
     IMapper mapper,
     IValidator<CreatePropertyAdRequest> createvalidator,
     IValidator<UpdatePropertyAdRequest> updatevalidator)
    {
        _repository = repository;
        _mediaRepository = mediaRepository;
        _fileStorage = fileStorage;
        _mapper = mapper;
        _createValidator = createvalidator;
        _updateValidator = updatevalidator;
    }
    public async Task<bool> CreatePropertyAdAsync(
     CreatePropertyAdRequest request,
     List<MediaUploadInput>? media,
     CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken: ct);

        var entity = _mapper.Map<PropertyAd>(request);

        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);

        if (media != null && media.Count > 0)
        {
            var order = 1;
            foreach (var item in media)
            {
                var objectKey = await _fileStorage.SaveAsync(
                    item.Content,
                    item.FileName,
                    item.ContentType,
                    entity.Id,
                    ct);

                var mediaEntity = new PropertyMedia
                {
                    PropertyAdId = entity.Id,
                    ObjectKey = objectKey,
                    Order = order++
                };

                await _mediaRepository.AddAsync(mediaEntity, ct);
            }

            await _mediaRepository.SaveChangesAsync(ct);
        }

        return true;
    }

    public async Task<bool> DeletePropertyAdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return false;

        var mediaList = await _mediaRepository.GetByPropertyAdIdAsync(id, ct);
        foreach (var m in mediaList)
        {
            await _fileStorage.DeleteFileAsync(m.ObjectKey, ct);
            await _mediaRepository.DeleteAsync(m, ct);
        }

        await _repository.DeleteAsync(entity, ct);
        var affected = await _repository.SaveChangesAsync(ct);
        return affected > 0;
    }
    public async Task<List<GetAllPropertyAdResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllWithMediaAsync(ct);
        return _mapper.Map<List<GetAllPropertyAdResponse>>(entities);
    }

    public async Task<GetByIdPropertyAdResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdWithMediaAsync(id, ct);
        return entity is null ? null : _mapper.Map<GetByIdPropertyAdResponse>(entity);
    }

    public async Task<bool> UpdatePropertyAdAsync(
     int id,
     UpdatePropertyAdRequest request,
     List<MediaUploadInput>? addMedia,
     int[]? removeMediaIds,
     CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken: ct);

        if (request.Id != 0 && request.Id != id)
            return false;

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return false;

        _mapper.Map(request, entity);
        await _repository.SaveChangesAsync(ct);

        if (removeMediaIds != null && removeMediaIds.Length > 0)
        {
            foreach (var mediaId in removeMediaIds.Distinct())
            {
                var m = await _mediaRepository.GetByIdAsync(mediaId, ct);
                if (m == null) continue;
                if (m.PropertyAdId != id) continue; // başqa elanın mediası ola bilər

                await _fileStorage.DeleteFileAsync(m.ObjectKey, ct);
                await _mediaRepository.DeleteAsync(m, ct);
            }

            await _mediaRepository.SaveChangesAsync(ct);
        }

        if (addMedia != null && addMedia.Count > 0)
        {
            var existing = await _mediaRepository.GetByPropertyAdIdAsync(id, ct);
            var order = existing.Count == 0 ? 1 : existing.Max(x => x.Order) + 1;

            foreach (var item in addMedia)
            {
                var objectKey = await _fileStorage.SaveAsync(
                    item.Content,
                    item.FileName,
                    item.ContentType,
                    id,
                    ct);

                var mediaEntity = new PropertyMedia
                {
                    PropertyAdId = id,
                    ObjectKey = objectKey,
                    Order = order++
                };

                await _mediaRepository.AddAsync(mediaEntity, ct);
            }

            await _mediaRepository.SaveChangesAsync(ct);
        }

        return true;
    }

}
