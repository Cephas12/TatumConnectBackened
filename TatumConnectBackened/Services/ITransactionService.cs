using TatumConnectBackened.Common.Models;
using TatumConnectBackened.DTOs;
using TatumConnectBackened.Responses;

public interface ITransactionService
{
    Task<ApiResponse<TransactionResponseDto>> PurchaseAsync(ProductPurchaseRequestDto request, CancellationToken ct = default);

    Task<ApiResponse<PagedResult<TransactionDto>>> GetTransactionsAsync(PaginationParameters pagination, TransactionFilterDto? filter = null);

    Task<ApiResponse<TatumConnectBackened.DTOs.TransactionDto>> GetTransactionByIdAsync(Guid id);

    Task<ApiResponse<TransactionSummaryDto>> GetAdminTransactionSummaryAsync(TransactionSummaryFilterDto filter);
}
