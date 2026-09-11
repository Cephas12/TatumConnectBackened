//using Microsoft.Identity.Client.NativeInterop;
using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Repositories
{
    public interface IAccountRepository: IRepository<Account>
    {
        Task<List<Account>> GetFilteredAsync(
            Guid? id = null,
            Guid? customerId = null,
            string? accountNumber = null);

        Task<IEnumerable<Account>> GetByCustomerIdsAsync(
            IEnumerable<Guid> customerIds,
            CancellationToken ct = default);

        Task<PagedResult<Account>>
            GetPagedAsync(
              Guid? id,
              Guid? customerId,
              string? accountNumber,
              PaginationParameters pagination,
              CancellationToken ct = default);

    }
}
