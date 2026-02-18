using Application.Dtos.AuthDtos;
using Application.Shared.Helpers.Responses;

namespace Application.Abstracts.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<BaseResponse<TokenResponse?>> LoginAsync(LoginRequest request, CancellationToken ct = default);
     Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<bool> ConfirmEmailAsync(
       string userId,
       string token,
       CancellationToken cancellationToken = default);

}
