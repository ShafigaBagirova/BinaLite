using Application.Abstracts.Services;
using Application.Shared.Helpers.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyModerationController : ControllerBase
{
    private readonly IPropertyModerationService _moderation;

    public PropertyModerationController(IPropertyModerationService moderation)
    {
        _moderation = moderation;
    }

    [HttpPost("{id:int}/approve")]
    public async Task<ActionResult<BaseResponse>> Approve(int id, CancellationToken ct)
    {
        await _moderation.ApproveAsync(id, ct);
        return Ok(BaseResponse.Ok("Approved"));
    }

    public sealed class RejectRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult<BaseResponse>> Reject(int id, [FromBody] RejectRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return BadRequest(BaseResponse.Fail("Reason is required"));

        await _moderation.RejectAsync(id, request.Reason, ct);
        return Ok(BaseResponse.Ok("Rejected"));
    }
}
