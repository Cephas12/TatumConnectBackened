using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Common.Constants;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Repositories;

public interface IBillerRepository
{
    Task<PagedResult<Biller>> GetPagedAsync(
        BillerCategory? category,
        PaginationParameters pagination,
        CancellationToken ct = default);

    Task<Biller?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
