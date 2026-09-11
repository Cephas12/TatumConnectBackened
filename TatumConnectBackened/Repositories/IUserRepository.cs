using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Repositories
{
    // Change IRepository<User> to IRepository<UserModel>
    public interface IUserRepository : IRepository<User> 
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetUserWithTokensAsync(Guid userId);
        Task<User?> GetPasswordSetupTokenAsync(string token);
        Task<bool> ExistsByStaffIdAsync(string staffId);

        Task<PagedResult<User>> GetPagedAsync(
            Guid? userId,
            PaginationParameters pagination,
            CancellationToken ct = default);
    }
}
