using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Dtos.PropertyAdDtos;
using Application.Options;
using Application.Validations.PropertyAdValidation;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Persistence.Repositories;
using System.Linq;
using System.Security.Claims;

namespace Persistence.Services;

public class PropertyAdService : IPropertyAdService
{
    private readonly IPropertyAdRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePropertyAdRequest> _createValidator;
    private readonly IValidator<UpdatePropertyAdRequest> _updateValidator;
    private readonly IFileStorageService _fileStorage;
    private readonly IPropertyMediaRepository _mediaRepository;
    private readonly IEmailService _email;
    private readonly EmailOptions _emailOptions;
    private readonly IHttpContextAccessor _http;

    public PropertyAdService(
     IPropertyAdRepository repository,
     IPropertyMediaRepository mediaRepository,
     IFileStorageService fileStorage,
     IMapper mapper,
     IValidator<CreatePropertyAdRequest> createvalidator,
     IValidator<UpdatePropertyAdRequest> updatevalidator,IEmailService emailService,
    IOptions<EmailOptions> emailOptions, IHttpContextAccessor http)
    {
        _repository = repository;
        _mediaRepository = mediaRepository;
        _fileStorage = fileStorage;
        _mapper = mapper;
        _createValidator = createvalidator;
        _updateValidator = updatevalidator;
        _email = emailService;
        _emailOptions = emailOptions.Value;
        _http = http;
    }
    private (string? userId, bool isAdmin) CurrentUser()
    {
        var user = _http.HttpContext?.User;
        if (user is null) return (null, false);

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = user.IsInRole(RoleNames.Admin);

        return (userId, isAdmin);
    }

    public async Task<bool> CreatePropertyAdAsync(string UserId,
     CreatePropertyAdRequest request,
     List<MediaUploadInput>? media,
     CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken: ct);

        if (media != null && media.Count > 10)
            throw new ValidationException("Maximum 10 media allowed.");

        var entity = _mapper.Map<PropertyAd>(request);

        entity.UserId = UserId;
        entity.Status = PropertyStatus.Pending;
        entity.RejectionReason = null;

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

    public async Task<bool> DeletePropertyAdAsync(int id, string UserId, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return false;

        if (entity.UserId != UserId)
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

        entities = entities
            .Where(x => x.Status == PropertyStatus.Approved)
            .ToList();

        return _mapper.Map<List<GetAllPropertyAdResponse>>(entities);
    }

    public async Task<GetByIdPropertyAdResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdWithMediaAsync(id, ct);
        if (entity is null) return null;

        if (entity.Status == PropertyStatus.Approved)
            return _mapper.Map<GetByIdPropertyAdResponse>(entity);

        var (userId, isAdmin) = CurrentUser();
        if (isAdmin) return _mapper.Map<GetByIdPropertyAdResponse>(entity);

        if (!string.IsNullOrWhiteSpace(userId) &&
            string.Equals(entity.UserId, userId, StringComparison.Ordinal))
            return _mapper.Map<GetByIdPropertyAdResponse>(entity);

        return null;
    }

    public async Task<bool> UpdatePropertyAdAsync( int id,string UserId,
     UpdatePropertyAdRequest request,
     List<MediaUploadInput>? addMedia,
     int[]? removeMediaIds,
     CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken: ct);

        if (request.Id != 0 && request.Id != id)
            return false;

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return false;

        if (!string.Equals(entity.UserId, UserId, StringComparison.Ordinal))
            return false;

        _mapper.Map(request, entity);

        await _repository.SaveChangesAsync(ct);

        if (removeMediaIds is { Length: > 0 })
        {
            foreach (var mediaId in removeMediaIds.Distinct())
            {
                var m = await _mediaRepository.GetByIdAsync(mediaId, ct);
                if (m is null) continue;

                if (m.PropertyAdId != id) continue;

                await _fileStorage.DeleteFileAsync(m.ObjectKey, ct);
                await _mediaRepository.DeleteAsync(m, ct);
            }

            await _mediaRepository.SaveChangesAsync(ct);
        }

        if (addMedia is { Count: > 0 })
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
    public async Task<List<GetAllPropertyAdResponse>> GetMyAsync(
    string userId,
    CancellationToken ct = default)
    {
        var entities = await _repository.GetByOwnerWithMediaAsync(userId, ct);

        return _mapper.Map<List<GetAllPropertyAdResponse>>(entities);
    }

}
