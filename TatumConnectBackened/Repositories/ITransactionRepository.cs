using TatumConnectBackened.Common.Models;
using E = TatumConnectBackened.Entities;

namespace TatumConnectBackened.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(E.Transaction transaction);

        Task<PagedResult<E.Transaction>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Guid? transactionId = null,
            Guid? userId = null,
            Guid? accountId = null,
            string? accountNumber = null,
            Guid? billerId = null,
            Guid? productId = null,
            string? status = null,
            E.ProductCategory? category = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<E.Transaction?> GetByIdWithDetailsAsync(Guid id);
    }
}
