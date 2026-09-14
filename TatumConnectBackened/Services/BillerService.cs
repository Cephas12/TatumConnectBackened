using TatumConnectBackened.Common.Models;
using TatumConnectBackened.DTOs;
using TatumConnectBackened.Entities;
using TatumConnectBackened.Repositories;
using TatumConnectBackened.Responses;

namespace TatumConnectBackened.Services;

public class BillerService : IBillerService
{
    private readonly IBillerRepository _repository;

    public BillerService(IBillerRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<PagedResult<BillerDto>>> GetAsync(
        BillerQueryParameters query,
        CancellationToken ct = default)
    {
        var result = await _repository.GetPagedAsync(query.Category, query, ct);
        var data = new PagedResult<BillerDto>
        {
            Items = result.Items.Select(Map).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages
        };

        return ApiResponse<PagedResult<BillerDto>>.Ok(data);
    }

    public async Task<ApiResponse<BillerDto>> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var biller = await _repository.GetByIdAsync(id, ct);
        return biller is null
            ? ApiResponse<BillerDto>.Fail(
                "Biller was not found.",
                new List<ApiError> { new("BillerNotFound", $"No biller exists with id '{id}'.") })
            : ApiResponse<BillerDto>.Ok(Map(biller));
    }

    private static BillerDto Map(Biller biller) => new()
    {
        Id = biller.Id,
        Name = biller.Name,
        Code = biller.Code,
        Category = biller.Category,
        IsActive = biller.IsActive
    };
}
