using System.Linq.Expressions;
using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Repositories
{
    public interface IRepository<T> where T : class 
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void update(T entity);
        Task UpdateAsync(T entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync(
            Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAync(
            Expression<Func<T, bool>> predicate);

        //Pagination

        Task<PagedResult<T>> GetPagedAsync(
            int pageNumber = 1, 
            int pageSize = 10,
            CancellationToken ct = default);

        Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken ct = default);

        Task<int> SavedChangesAsync(
            CancellationToken ct = default);

    }
}
