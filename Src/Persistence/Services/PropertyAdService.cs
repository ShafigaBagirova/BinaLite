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

    public PropertyAdService(IPropertyAdRepository repository,IMapper mapper,
        IValidator<CreatePropertyAdRequest> createvalidator,
        IValidator<UpdatePropertyAdRequest> updatevalidator)
    {

        _repository = repository;
        _mapper = mapper;
        _createValidator = createvalidator;
        _updateValidator = updatevalidator;
    }
    public async Task<bool> CreatePropertyAdAsync(CreatePropertyAdRequest request, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken: ct);

        var entity = _mapper.Map<PropertyAd>(request);
        await _repository.AddAsync(entity, ct);

        var affected = await _repository.SaveChangesAsync(ct);
        return affected > 0;
    }

    public async Task<bool> DeletePropertyAdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return false;

        await _repository.DeleteAsync(entity, ct);
        var affected = await _repository.SaveChangesAsync(ct);
        return affected > 0;
    }
    public async Task<List<GetAllPropertyAdResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct);
        return _mapper.Map<List<GetAllPropertyAdResponse>>(entities);
    }

    public async Task<GetByIdPropertyAdResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<GetByIdPropertyAdResponse>(entity);
    }

    public async Task<bool> UpdatePropertyAdAsync(int id, UpdatePropertyAdRequest request, CancellationToken ct = default)
    {

        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken: ct);
        
        if (request.Id != 0 && request.Id != id)
            return false;

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return false; 

        _mapper.Map(request, entity);

        var affected = await _repository.SaveChangesAsync(ct);
        return affected > 0;
    }

}
