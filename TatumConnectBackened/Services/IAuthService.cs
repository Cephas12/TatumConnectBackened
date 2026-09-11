using TatumConnectBackened.DTOs;
using TatumConnectBackened.Responses;

namespace TatumConnectBackened.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<UserDto>> RegisterAsync(
    RegisterRequestDto request,
    CancellationToken ct = default);
        Task<ApiResponse<UserDto>> VerifyRegistrationOtpAsync(
            VerifyRegistrationOtpRequestDto request,
            CancellationToken ct = default);

        Task<ApiResponse<LoginResponseDto>> LoginAsync(
            LoginRequestDto request,
            CancellationToken ct = default);
        Task<ApiResponse<UserDto>> ResendRegistrationOtpAsync(
            ResendRegistrationOtpRequestDto request,
            CancellationToken ct = default);
    }
}