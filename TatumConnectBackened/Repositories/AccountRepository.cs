using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Data;
using TatumConnectBackened.Entities;
using EntityAccount = TatumConnectBackened.Entities.Account;

namespace TatumConnectBackened.Repositories
{
    public class AccountRepository : Repository<EntityAccount>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<EntityAccount>> GetFilteredAsync(
            Guid? id = null,
            Guid? customerId = null,
            string? accountNumber = null)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(a => a.Id == id.Value);
            }
            if (customerId.HasValue)
            {
                query = query.Where(a => a.CustomerId == customerId.Value);
            }
            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                query = query.Where(a => a.AccountNumber == accountNumber);
            }
            return await query.OrderBy(a => a.AccountNumber).ToListAsync();
        }

        public async Task<IEnumerable<EntityAccount>> GetByCustomerIdsAsync(
            IEnumerable<Guid> customerIds,
            CancellationToken ct = default)
        {
            var ids = customerIds.ToList();
            if (!ids.Any())
            {
                return Enumerable.Empty<EntityAccount>();
            }
            return await _dbSet
                .AsNoTracking()
                .Where(a => ids.Contains(a.CustomerId))
                .ToListAsync(ct);
        }

        public async Task<PagedResult<EntityAccount>> GetPagedAsync(
            Guid? id,
            Guid? customerId,
            string? accountNumber,
            PaginationParameters pagination,
            CancellationToken ct = default)
        {
            var query = _context.Accounts.AsNoTracking().AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(a => a.Id == id.Value);
            }

            if (customerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == customerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                var number = accountNumber.Trim();
                query = query.Where(x => x.AccountNumber == number);
            }

            var totalCount = await query.CountAsync(ct);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync(ct);

            return new PagedResult<EntityAccount>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}