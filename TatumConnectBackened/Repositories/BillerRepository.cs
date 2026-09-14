using Microsoft.EntityFrameworkCore;
using TatumConnectBackened.Common.Constants;
using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Data;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Repositories;

public class BillerRepository : IBillerRepository
{
    private readonly AppDbContext _context;

    public BillerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Biller>> GetPagedAsync(
        BillerCategory? category,
        PaginationParameters pagination,
        CancellationToken ct = default)
    {
        var pageNumber = pagination.PageNumber < 1 ? 1 : pagination.PageNumber;
        var query = _context.Billers.AsNoTracking();
        if (category.HasValue)
        {
            query = query.Where(biller => biller.Category == category.Value);
        }

        var totalCount = await query.CountAsync(ct);
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)pagination.PageSize);
        var items = await query
            .OrderBy(biller => biller.Name)
            .Skip((pageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Biller>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public Task<Biller?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _context.Billers
            .AsNoTracking()
            .FirstOrDefaultAsync(biller => biller.Id == id, ct);
    }
}
