using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Entities;
using Persistence.Repositories;
using System.Linq;

namespace Persistence.Services;

public class PropertyAdService : IPropertyAdService
{
    private readonly IPropertyAdRepository _repository;
    private readonly IMapper _mapper;

    public PropertyAdService(IPropertyAdRepository repository,IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task CreatePropertyAdAsync(CreatePropertyAdRequest request, CancellationToken ct = default)
    {
      var entity = _mapper.Map<PropertyAd>(request);
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task DeletePropertyAdAsync(int Id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(Id, ct);
        if (entity == null)
            throw new KeyNotFoundException($"PropertyAd with Id={Id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task<List<GetAllPropertyAdResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct);
        return _mapper.Map<List<GetAllPropertyAdResponse>>(entities);
    }

    public async Task<GetByIdPropertyAdResponse> GetByIdAsync(int Id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(Id, ct);
        if (entity == null)
        {
            return null;
        }

        return _mapper.Map<GetByIdPropertyAdResponse>(entity);
    }

    public async Task UpdatePropertyAdAsync(int Id, UpdatePropertyAdRequest request, CancellationToken ct = default)
    {


        var entity = await _repository.GetByIdAsync(Id, ct);
        if (entity == null)
            throw new KeyNotFoundException($"PropertyAd {Id} not found");

        _mapper.Map(request, entity);
        await _repository.SaveChangesAsync(ct);
    }
        
}
