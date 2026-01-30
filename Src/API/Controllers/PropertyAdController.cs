using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Dtos;
using Application.Shared.Helpers.Responses;
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
    public async Task<ActionResult<BaseResponse<List<GetAllPropertyAdResponse>>>> GetAllAsync(CancellationToken ct)
    {
        var propertyAds = await _propertyAdService.GetAllAsync(ct);
        return Ok(BaseResponse<List<GetAllPropertyAdResponse>>.Ok(propertyAds));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseResponse<GetByIdPropertyAdResponse>>> GetByIdAsync(int id, CancellationToken ct)
    {
        var propertyAd = await _propertyAdService.GetByIdAsync(id, ct);
         if(propertyAd is null)
            return NotFound(BaseResponse<GetByIdPropertyAdResponse>.Fail("PropertyAd not found"));
        return Ok(BaseResponse<GetByIdPropertyAdResponse>.Ok(propertyAd));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse>> CreateAsync(CreatePropertyAdRequest request, CancellationToken ct)
    {
        var ok=await _propertyAdService.CreatePropertyAdAsync(request, ct);
        if (!ok) return BadRequest(BaseResponse.Fail("Could not create PropertyAd"));
        return Ok(BaseResponse.Ok("Created succesfully")); 
    }

  
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BaseResponse>> UpdateAsync(int id, UpdatePropertyAdRequest request, CancellationToken ct)
    {
        var ok = await _propertyAdService.UpdatePropertyAdAsync(id, request, ct);

        if (!ok)
            return NotFound(BaseResponse.Fail("PropertyAd not found"));

        return Ok(BaseResponse.Ok("Updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(int id, CancellationToken ct)
    {
        var ok = await _propertyAdService.DeletePropertyAdAsync(id, ct);

        if (!ok)
            return NotFound(BaseResponse.Fail("PropertyAd not found"));

        return Ok(BaseResponse.Ok("Deleted successfully"));
    }


}
