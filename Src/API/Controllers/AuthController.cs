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
                        ? new List<string> { error }
                        : null
                )
            );
        }

        return Ok(BaseResponse.Ok("User registered successfully"));
    }

    // POST: /api/Auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var token = await _authService.LoginAsync(request, ct);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(
                BaseResponse<string>.Fail("Invalid login or password.")
            );
        }

        return Ok(
            BaseResponse<string>.Ok(
                data: token,
                message: "Login successful"
            )
        );
    }
}
