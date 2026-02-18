using Application.Abstracts.Services;
using Application.Dtos.AuthDtos;
using Application.Options;
using Application.Shared.Helpers.Responses;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net;

namespace Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtOptions _jwtOptions;

    private readonly IEmailService _emailService;
    private readonly EmailOptions _emailOptions;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtGenerator,
        IRefreshTokenService refreshTokenService,
        IOptions<JwtOptions> jwtOptions,
        IEmailService emailService,
        IOptions<EmailOptions> emailOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _refreshTokenService = refreshTokenService;
        _jwtOptions = jwtOptions.Value;

        _emailService = emailService;
        _emailOptions = emailOptions.Value;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(
        RegisterRequest request,
        CancellationToken ct = default)
    {
        var user = new User
        {
            UserName = request.UserName?.Trim(),
            Email = request.Email?.Trim(),
            FullName = request.FullName?.Trim()
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var message = string.Join(" | ", createResult.Errors.Select(e => e.Description));
            return (false, message);
        }

        try
        {
            var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.User);
            if (!roleResult.Succeeded)
            {
                var message = string.Join(" | ", roleResult.Errors.Select(e => e.Description));

                await _userManager.DeleteAsync(user);

                return (false, message);
            }

            var baseUrl = (_emailOptions.ConfirmationBaseUrl ?? string.Empty).TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                await _userManager.DeleteAsync(user);
                return (false, "Email ConfirmationBaseUrl is not configured.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var confirmationLink = QueryHelpers.AddQueryString(baseUrl, new Dictionary<string, string?>
            {
                ["userId"] = user.Id,
                ["token"] = encodedToken
            });

            var subject = "Confirm your email";
            var htmlBody = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.";
            var plainTextBody = $"Please confirm your email by visiting the following link: {confirmationLink}";

            await _emailService.SendAsync(user.Email!, subject, htmlBody, plainTextBody, ct);

            return (true, null);
        }
        catch (OperationCanceledException)
        {
          
            await SafeDeleteUserAsync(user);
            return (false, "Registration was canceled.");
        }
        catch (Exception ex)
        {
            await SafeDeleteUserAsync(user);

            return (false, $"Registration failed: {ex.Message}");
        }
    }

    private async Task SafeDeleteUserAsync(User user)
    {
        try
        {
            
            if (!string.IsNullOrWhiteSpace(user?.Id))
                await _userManager.DeleteAsync(user);
        }
        catch
        {
            // burada log yazmaq daha yaxşıdır, amma user-facing error-u dəyişməyək
        }
    }

    private async Task<TokenResponse> BuildTokenResponseAsync(User user, CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtGenerator.GenerateAccessToken(user, roles);

        var refreshToken = await _refreshTokenService.CreateAsync(user, ct);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public async Task<BaseResponse<TokenResponse?>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Login)
            ?? await _userManager.FindByNameAsync(request.Login);

        if (user is null)
            return BaseResponse<TokenResponse?>.Fail("InvalidCredentials");

        var ok = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!ok)
            return BaseResponse<TokenResponse?>.Fail("InvalidCredentials");

        if (!user.EmailConfirmed)
            return BaseResponse<TokenResponse?>.Fail("EmailNotConfirmed");

        var tokenResponse = await BuildTokenResponseAsync(user, ct);
        return BaseResponse<TokenResponse?>.Ok(tokenResponse, "Login successful.");
    }

    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var user = await _refreshTokenService.ValidateAndConsumeAsync(refreshToken, ct);
        if (user is null)
            return null;

        return await BuildTokenResponseAsync(user, ct);
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;

        var decodedToken = WebUtility.UrlDecode(token);
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded;
    }
}
