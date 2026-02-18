
using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Dtos.PropertyAdDtos;
using Application.Shared.Helpers.Responses;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyAdController : ControllerBase
{
    private readonly IPropertyAdService _propertyAdService;
    private readonly IPropertyMediaRepository _mediaRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly IMapper _mapper;
    public PropertyAdController(
     IPropertyAdService propertyAdService,
     IPropertyMediaRepository mediaRepository,
     IFileStorageService fileStorage,
     IMapper mapper)
    {
        _propertyAdService=propertyAdService;
        _mediaRepository = mediaRepository;
        _fileStorage = fileStorage;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<ActionResult<BaseResponse<List<GetAllPropertyAdResponse>>>> GetAllAsync(CancellationToken ct)
    {
        var propertyAds = await _propertyAdService.GetAllAsync(ct);
        return Ok(BaseResponse<List<GetAllPropertyAdResponse>>.Ok(propertyAds));
    }
    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<BaseResponse<List<GetAllPropertyAdResponse>>>> GetMy(CancellationToken ct)
    {
        var result = await _propertyAdService.GetMyAsync(ct);
        return Ok(BaseResponse<List<GetAllPropertyAdResponse>>.Ok(result));
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseResponse<GetByIdPropertyAdResponse>>> GetByIdAsync(int id, CancellationToken ct)
    {
        var propertyAd = await _propertyAdService.GetByIdAsync(id, ct);

    if (propertyAd is null)
        return NotFound(BaseResponse<GetByIdPropertyAdResponse>.Fail("PropertyAd not found"));

    return Ok(BaseResponse<GetByIdPropertyAdResponse>.Ok(propertyAd));
    }

    [Authorize(Policy = Policies.ManageProperties)]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BaseResponse>> Create(
        [FromForm] CreatePropertyAdRequest request,
        [FromForm] IFormFileCollection? media,
        CancellationToken ct)
    {
        List<MediaUploadInput>? inputs = null;

        try
        {
            if (media != null && media.Count > 0)
            {
                inputs = media.Select(f => new MediaUploadInput
                {
                    Content = f.OpenReadStream(),
                    FileName = f.FileName,
                    ContentType = f.ContentType ?? "application/octet-stream",
                    Length = f.Length
                }).ToList();
            }

            var ok = await _propertyAdService.CreatePropertyAdAsync(request, inputs, ct);

            if (!ok)
                return BadRequest(BaseResponse.Fail("Create failed"));

            return Ok(BaseResponse.Ok("Created successfully"));
        }
        finally
        {
            if (inputs != null)
                foreach (var i in inputs)
                    i.Content.Dispose();
        }
    }

    [Authorize(Policy = Policies.ManageProperties)]
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BaseResponse>> Update(
        int id,
        [FromForm] UpdatePropertyAdRequest request,
        [FromForm] IFormFileCollection? addMedia,
        [FromForm] int[]? removeMediaIds,
        CancellationToken ct)
    {
        List<MediaUploadInput>? inputs = null;

        try
        {
            if (addMedia != null && addMedia.Count > 0)
            {
                inputs = addMedia.Select(f => new MediaUploadInput
                {
                    Content = f.OpenReadStream(),
                    FileName = f.FileName,
                    ContentType = f.ContentType ?? "application/octet-stream",
                    Length = f.Length
                }).ToList();
            }

            var ok = await _propertyAdService.UpdatePropertyAdAsync(
                id, request, inputs, removeMediaIds, ct);

            if (!ok)
                return NotFound(BaseResponse.Fail("PropertyAd not found or not allowed"));

            return Ok(BaseResponse.Ok("Updated successfully"));
        }
        finally
        {
            if (inputs != null)
                foreach (var i in inputs)
                    i.Content.Dispose();
        }
    }

    [Authorize(Policy = Policies.ManageProperties)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(int id, CancellationToken ct)
    {
        var ok = await _propertyAdService.DeletePropertyAdAsync(id, ct);

        if (!ok)
            return NotFound(BaseResponse.Fail("PropertyAd not found or not allowed"));

        return Ok(BaseResponse.Ok("Deleted successfully"));
    }


    [HttpGet("{propertyId:int}/media")]
    public async Task<ActionResult<BaseResponse<List<PropertyAdMediaItemDto>>>> GetMedia(int propertyId, CancellationToken ct)
    {
        var list = await _mediaRepository.GetByPropertyAdIdAsync(propertyId, ct);
        var dto = _mapper.Map<List<PropertyAdMediaItemDto>>(list);

        return Ok(BaseResponse<List<PropertyAdMediaItemDto>>.Ok(dto));
    }

    [Authorize(Policy =Policies.ManageProperties)]
    [HttpDelete("media/{id:int}")]
    public async Task<ActionResult<BaseResponse>> DeleteMedia(int id, CancellationToken ct)
    {
        var media = await _mediaRepository.GetByIdAsync(id, ct);
        if (media == null)
            return NotFound(BaseResponse.Fail("Media not found"));

        await _fileStorage.DeleteFileAsync(media.ObjectKey, ct);
        await _mediaRepository.DeleteAsync(media, ct);
        await _mediaRepository.SaveChangesAsync(ct);

        return Ok(BaseResponse.Ok("Media deleted successfully"));
    }


}
