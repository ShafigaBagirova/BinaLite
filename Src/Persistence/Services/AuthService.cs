using Application.Abstracts.Services;
using Application.Dtos.AuthDtos;
using Application.Options;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtOptions _jwtOptions;


    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtGenerator,
        IRefreshTokenService refreshTokenService,
       IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _refreshTokenService = refreshTokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(
        RegisterRequest request,
        CancellationToken ct = default)
    {

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var message = string.Join(" | ", result.Errors.Select(e => e.Description));
            return (false, message);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.User);

        if (!roleResult.Succeeded)
        {
            var message = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
            return (false, message);
        }

        return (true, null);
    }
    private async Task<TokenResponse> BuildTokenResponseAsync(User user, CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);

        // 2) Access token
        var accessToken = _jwtGenerator.GenerateAccessToken(user, roles);

        // 3) Refresh token (DB-yə yazılan/rotasiya olunan)
        var refreshToken = await _refreshTokenService.CreateAsync(user, ct);

        // 4) ExpiresAtUtc (Access token müddəti)
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public async Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
       
        var user = await _userManager.FindByEmailAsync(request.Login)
            ?? await _userManager.FindByNameAsync(request.Login);
        if (user is null)
            return null;

        var ok = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!ok)
            return null;

        return await BuildTokenResponseAsync(user, ct);
    }
    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var user = await _refreshTokenService.ValidateAndConsumeAsync(refreshToken, ct);
        if (user is null)
            return null;

        return await BuildTokenResponseAsync(user, ct);
    }

}

