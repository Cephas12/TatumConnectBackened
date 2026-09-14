using TatumConnectBackened.Common.Models;
using TatumConnectBackened.DTOs;
using TatumConnectBackened.Responses;

namespace TatumConnectBackened.Services;

public interface IBillerService
{
    Task<ApiResponse<PagedResult<BillerDto>>> GetAsync(
        BillerQueryParameters query,
        CancellationToken ct = default);

    Task<ApiResponse<BillerDto>> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);
}
