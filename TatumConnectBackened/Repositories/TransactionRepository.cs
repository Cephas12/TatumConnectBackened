using Microsoft.EntityFrameworkCore;
using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Data;
using E = TatumConnectBackened.Entities;

namespace TatumConnectBackened.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(E.Transaction transaction)
        {
            await _context.Set<E.Transaction>().AddAsync(transaction);
        }

        public async Task<PagedResult<E.Transaction>> GetPagedAsync(
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
            DateTime? toDate = null)
        {
            var query = _context.Set<E.Transaction>()
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.Account)
                .Include(t => t.Biller)
                .Include(t => t.Product)
                .Include(t => t.ProductItem)
                .AsQueryable();

            if (transactionId.HasValue)
                query = query.Where(t => t.Id == transactionId.Value);

            if (userId.HasValue)
                query = query.Where(t => t.CustomerId == userId.Value);

            if (accountId.HasValue)
                query = query.Where(t => t.AccountId == accountId.Value);

            if (!string.IsNullOrWhiteSpace(accountNumber))
                query = query.Where(t => t.Account != null && t.Account.AccountNumber == accountNumber);

            if (billerId.HasValue)
                query = query.Where(t => t.BillerId == billerId.Value);

            if (productId.HasValue)
                query = query.Where(t => t.ProductId == productId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status == status);

            if (category.HasValue)
                query = query.Where(t => t.Type == category.Value.ToString());

            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt < toDate.Value.AddDays(1));

            var totalCount = await query.CountAsync();
            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<E.Transaction>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<E.Transaction?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Set<E.Transaction>()
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.Account)
                .Include(t => t.Biller)
                .Include(t => t.Product)
                .Include(t => t.ProductItem)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
