using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyAdController : ControllerBase
{
    private readonly IPropertyAdService _propertyAdService;
    public PropertyAdController(IPropertyAdService propertyAdService)
    {
        _propertyAdService = propertyAdService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var propertyAds = await _propertyAdService.GetAllAsync(ct);
        return Ok(propertyAds);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var propertyAd = await _propertyAdService.GetByIdAsync(id, ct);
        if (propertyAd == null) return NotFound();
        return Ok(propertyAd);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePropertyAdRequest request, CancellationToken ct)
    {
        await _propertyAdService.CreatePropertyAdAsync(request, ct);
        return Ok(); 
    }

  
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePropertyAdRequest request, CancellationToken ct)
    {
        try
        {
            await _propertyAdService.UpdatePropertyAdAsync(id, request, ct);
            return NoContent(); 
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _propertyAdService.DeletePropertyAdAsync(id, ct);
            return NoContent(); // 204
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


}
