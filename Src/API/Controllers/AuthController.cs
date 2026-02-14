using Application.Abstracts.Services;
using Application.Dtos.AuthDtos;
using Application.Shared.Helpers.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: /api/Auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var (success, error) = await _authService.RegisterAsync(request, ct);

        if (!success)
        {
            return BadRequest(
                BaseResponse.Fail(
                    message: "Register failed",
                    errors: error is not null
                        ? new List<string> { error }  : null
                )
            );
        }

        return Ok(BaseResponse.Ok("User registered successfully"));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<TokenResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        if (result is null)
            return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid username or password"));
        return Ok(BaseResponse<TokenResponse>.Ok(result));

    }
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<TokenResponse>>> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(
                BaseResponse<TokenResponse>.Fail("RefreshToken is required."));


        var result = await _authService.RefreshTokenAsync(request.RefreshToken, ct);
        if (result is null)
             return Unauthorized(
        BaseResponse<TokenResponse>.Fail("Invalid or expired refresh token")
    );

        return Ok(BaseResponse<TokenResponse>.Ok(result));
    }
}
