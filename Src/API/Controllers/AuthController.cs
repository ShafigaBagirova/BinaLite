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
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(
       [FromQuery] string? userId,
       [FromQuery] string? token,
       CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            return BadRequest("userId and token are required.");

        var success = await _authService.ConfirmEmailAsync(userId, token, ct);

        if (!success)
            return BadRequest("Token is invalid or expired.");

        return Ok("Email confirmed.");
    }

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

        if (!result.Success)
        {
            if (result.Message == "EmailNotConfirmed")
                return StatusCode(403, BaseResponse.Fail("Email is not confirmed. Please confirm your email."));

            return Unauthorized(BaseResponse.Fail(result.Message ?? "Invalid username or password"));
        }

        return Ok(result);

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
