using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Data;
using TatumConnectBackened.Entities;


namespace TatumConnectBackened.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context)
            : base(context) 
        { 

        }
        public async Task<User?> GetByEmailAsync(
            string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(
                u => u.Email == email);
        }
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }
        public async Task<User?>
            GetUserWithTokensAsync(Guid userId)
        {
            return await _dbSet.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<User?> GetPasswordSetupTokenAsync(string token)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .FirstOrDefaultAsync(
                    u =>
                        u.PasswordSetupToken == token &&
                        u.PasswordSetupTokenExpiresAt > now);
        }
        public async Task<bool> ExistsByStaffIdAsync(string staffId)
        {
            return await _dbSet.AnyAsync(u => u.StaffId == staffId);
        }

        public async Task<PagedResult<User>> GetPagedAsync(Guid? userId, PaginationParameters pagination, CancellationToken ct = default)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            //Filter by user ID

            if (userId.HasValue)
            {
                query = query.Where(x => x.Id == userId);
            }

            //Total Records
            var totalCount = await query.CountAsync(ct);

            //caluclate total pages
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            //Get current page

            var items =
                await query
                      .OrderByDescending(
                    x => x.CreatedAt)
                      .Skip(
                    (pagination.PageNumber - 1) * pagination.PageSize)
                      .Take(
                    pagination.PageSize).ToListAsync(ct);

            return new PagedResult<User>
            {
                Items = items,

                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
        }
    }
}
