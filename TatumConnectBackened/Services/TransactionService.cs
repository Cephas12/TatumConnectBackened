using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TatumConnectBackened.Common.Models;
using TatumConnectBackened.Common.Constants;
using TatumConnectBackened.DTOs;
using E = TatumConnectBackened.Entities;
using TatumConnectBackened.Repositories;
using TatumConnectBackened.Responses;
using TatumConnectBackened.Data;

namespace TatumConnectBackened.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accounts;
        private readonly ITransactionRepository _transactions;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _context;

        public TransactionService(
            AppDbContext context,
            IAccountRepository accounts,
            ITransactionRepository transactions,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _accounts = accounts;
            _transactions = transactions;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<TransactionResponseDto>> PurchaseAsync(ProductPurchaseRequestDto request, CancellationToken ct = default)
        {
            var currentUserId = _currentUserService.UserId;

            if (currentUserId == Guid.Empty)
            {
                return Fail("Unable to identify the authenticated user.", "InvalidUser");
            }

            var account = await _accounts.GetByIdAsync(request.AccountId);
            if (account == null)
            {
                return Fail("Account not found.", "AccountNotFound");
            }

            if (account.CustomerId != currentUserId)
            {
                return ApiResponse<TransactionResponseDto>.Fail("You are not authorized to use this account.", new List<ApiError>
                {
                    new("UnauthorizedAccount","The specified account does not belong to the authenticated user.")
                });
            }

            // Load product and item using DbContext directly
            var product = await _context.Set<E.Product>()
                .Include(p => p.ProductItems)
                .Include(p => p.Biller)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

            if (product == null || !product.IsActive)
            {
                return Fail("Product not found or inactive.", "ProductNotFound");
            }

            var validation = ValidateRequiredFields(product, request.Fields);
            if (!validation.Success)
            {
                return ApiResponse<TransactionResponseDto>.Fail(validation.Message, validation.Errors);
            }

            decimal amount;
            E.ProductItem? item = null;

            if (request.ProductItemId.HasValue)
            {
                item = product.ProductItems.FirstOrDefault(x => x.Id == request.ProductItemId.Value && x.IsActive);
                if (item == null)
                {
                    return Fail("Product item not found.", "ProductItemNotFound");
                }
                amount = item.UnitPrice;
            }
            else
            {
                if (!product.AllowsCustomAmount)
                {
                    return Fail("A product item must be selected.", "ProductItemRequired");
                }
                if (!request.Amount.HasValue || request.Amount.Value <= 0)
                {
                    return Fail("A valid amount is required.", "InvalidAmount");
                }
                amount = request.Amount.Value;
            }

            if (account.AvailableBalance < amount)
            {
                return Fail("Insufficient account balance.", "InsufficientBalance");
            }

            var transaction = new E.Transaction
            {
                Id = Guid.NewGuid(),
                CustomerId = currentUserId,
                Reference = GenerateReference(),
                Type = product.Category.ToString(),
                Status = TransactionStatus.Pending.ToString(),
                Amount = amount,
                Currency = account.Currency,
                AccountId = request.AccountId,
                BillerId = product.BillerId,
                ProductId = product.Id,
                ProductItemId = item?.Id,
                Narration = null,
                CreatedAt = DateTime.UtcNow
            };

            account.AvailableBalance -= amount;
            account.LedgerBalance -= amount;

            await _transactions.AddAsync(transaction);
            // No-op placeholder update: file touched to trigger downstream processing.
            await _accounts.UpdateAsync(account);
            await _context.SaveChangesAsync(ct);

            var response = new TransactionResponseDto
            {
                Id = transaction.Id,
                Reference = transaction.Reference,
                Type = transaction.Type,
                Status = transaction.Status,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                BillerName = transaction.Biller?.Name,
                ProductName = transaction.Product?.Name,
                ProductItemName = transaction.ProductItem?.Name,
                CreatedAt = transaction.CreatedAt,
                CompletedAt = transaction.CompletedAt
            };

            return ApiResponse<TransactionResponseDto>.Ok(response, "Transaction completed successfully.");
        }

        public async Task<ApiResponse<PagedResult<TransactionDto>>> GetTransactionsAsync(PaginationParameters pagination, TatumConnectBackened.DTOs.TransactionFilterDto? filter = null)
        {
            filter ??= new TatumConnectBackened.DTOs.TransactionFilterDto();

            var currentUserId = _currentUserService.UserId;

            var isAdmin = _currentUserService.IsAdmin || _currentUserService.IsSuperAdmin;

            if (!isAdmin)
            {
                if (currentUserId == Guid.Empty)
                {
                    return ApiResponse<PagedResult<TransactionDto>>.Fail("Unable to identify the current user.", new List<ApiError> { new("Unauthorized","The authenticated user could not be identified.") });
                }
                filter.UserId = currentUserId;
            }

            // Map DTO ProductCategory (string) to entity ProductCategory?
            E.ProductCategory? entityCategory = null;
            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                if (Enum.TryParse<E.ProductCategory>(filter.Category, true, out var parsedCat))
                {
                    entityCategory = parsedCat;
                }
            }

            var result = await _transactions.GetPagedAsync(
                pagination.PageNumber,
                pagination.PageSize,
                filter.TransactionId,
                filter.UserId,
                filter.AccountId,
                filter.AccountNumber,
                filter.BillerId,
                filter.ProductId,
                filter.Status?.ToString(),
                entityCategory,
                filter.FromDate,
                filter.ToDate);

            var dto = new PagedResult<TatumConnectBackened.DTOs.TransactionDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Items = result.Items.Select(MapTransactionToDto).ToList()
            };

            return ApiResponse<PagedResult<TatumConnectBackened.DTOs.TransactionDto>>.Ok(dto, "Transactions retrieved successfully.");
        }

        public async Task<ApiResponse<TatumConnectBackened.DTOs.TransactionDto>> GetTransactionByIdAsync(Guid id)
        {
            var transaction = await _transactions.GetByIdWithDetailsAsync(id);

            if (transaction == null)
            {
                return ApiResponse<TatumConnectBackened.DTOs.TransactionDto>.Fail("Transaction not found.", new List<ApiError> { new("NotFound", $"Transaction '{id}' was not found.") });
            }

            var isAdmin = _currentUserService.IsAdmin || _currentUserService.IsSuperAdmin;

            if (!isAdmin && transaction.CustomerId != _currentUserService.UserId)
            {
                return ApiResponse<TatumConnectBackened.DTOs.TransactionDto>.Fail("You are not authorized to view this transaction.", new List<ApiError> { new("Forbidden","You can only view your own transactions.") });
            }

            return ApiResponse<TatumConnectBackened.DTOs.TransactionDto>.Ok(MapTransactionToDto(transaction), "Transaction retrieved successfully.");
        }

        public async Task<ApiResponse<TatumConnectBackened.DTOs.TransactionSummaryDto>> GetAdminTransactionSummaryAsync(TatumConnectBackened.DTOs.TransactionSummaryFilterDto filter)
        {
            var isAdmin = _currentUserService.IsAdmin || _currentUserService.IsSuperAdmin;
            if (!isAdmin)
            {
                return ApiResponse<TatumConnectBackened.DTOs.TransactionSummaryDto>.Fail("You are not authorized to view transaction summaries.", new List<ApiError> { new("Forbidden","Only administrators can view transaction summaries.") });
            }

            if (filter.Period.HasValue && (filter.FromDate.HasValue || filter.ToDate.HasValue))
            {
                return ApiResponse<TatumConnectBackened.DTOs.TransactionSummaryDto>.Fail("Invalid date filters.", new List<ApiError> { new("InvalidDateFilter","Specify either Period or FromDate/ToDate, not both.") });
            }

            var (fromDate, toDate) = ResolveDateRange(filter);

            // Compute summary directly
            var query = _context.Transactions.AsNoTracking();
            var filtered = query.Where(t => t.CreatedAt >= fromDate && t.CreatedAt < toDate);

            var totalTransactions = await filtered.CountAsync();
            var successfulTransactions = await filtered.CountAsync(t => t.Status == TransactionStatus.Successful.ToString());
            var failedTransactions = await filtered.CountAsync(t => t.Status == TransactionStatus.Failed.ToString());
            var pendingTransactions = await filtered.CountAsync(t => t.Status == TransactionStatus.Pending.ToString());

            var totalAmount = await filtered.SumAsync(t => (decimal?)t.Amount) ?? 0m;
            var successfulAmount = await filtered.Where(t => t.Status == TransactionStatus.Successful.ToString()).SumAsync(t => (decimal?)t.Amount) ?? 0m;
            var failedAmount = await filtered.Where(t => t.Status == TransactionStatus.Failed.ToString()).SumAsync(t => (decimal?)t.Amount) ?? 0m;
            var pendingAmount = await filtered.Where(t => t.Status == TransactionStatus.Pending.ToString()).SumAsync(t => (decimal?)t.Amount) ?? 0m;

            var byCategory = await filtered.GroupBy(t => t.Type).Select(g => new { Category = g.Key ?? string.Empty, Count = g.Count(), TotalAmount = g.Sum(x => x.Amount) }).ToListAsync();

            var byBiller = await filtered
                .GroupBy(t => t.BillerId)
                .Select(g => new { BillerId = g.Key, Count = g.Count(), TotalAmount = g.Sum(x => x.Amount) })
                .ToListAsync();

            var summary = new TransactionSummaryDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalTransactions = totalTransactions,
                SuccessfulTransactions = successfulTransactions,
                FailedTransactions = failedTransactions,
                PendingTransactions = pendingTransactions,
                TotalAmount = totalAmount,
                SuccessfulAmount = successfulAmount,
                FailedAmount = failedAmount,
                PendingAmount = pendingAmount,
                ByCategory = byCategory.Select(c => new TransactionCategorySummaryDto { Category = c.Category, Count = c.Count, TotalAmount = c.TotalAmount, PercentageOfTransactions = totalTransactions == 0 ? 0 : Math.Round(c.Count * 100m / totalTransactions, 2) }).ToList(),
                ByBiller = new List<TransactionBillerSummaryDto>()
            };

            // Map biller details
            foreach (var b in byBiller)
            {
                var billerName = string.Empty;
                var billerCode = string.Empty;
                if (b.BillerId.HasValue)
                {
                    var biller = await _context.Set<E.Biller>().FindAsync(b.BillerId.Value);
                    billerName = biller?.Name ?? string.Empty;
                    billerCode = biller?.Code ?? string.Empty;
                }
                summary.ByBiller.Add(new TransactionBillerSummaryDto
                {
                    BillerId = b.BillerId,
                    BillerName = billerName,
                    BillerCode = billerCode,
                    Count = b.Count,
                    TotalAmount = b.TotalAmount,
                    PercentageOfTransactions = totalTransactions == 0 ? 0 : Math.Round(b.Count * 100m / totalTransactions, 2)
                });
            }

            return ApiResponse<TatumConnectBackened.DTOs.TransactionSummaryDto>.Ok(summary, "Transaction summary retrieved successfully.");
        }

        private static TatumConnectBackened.DTOs.TransactionDto MapTransactionToDto(E.Transaction transaction)
        {
            return new TatumConnectBackened.DTOs.TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.CustomerId,
                UserName = transaction.User != null ? $"{transaction.User.FirstName} {transaction.User.LastName}" : null,
                AccountId = transaction.AccountId,
                AccountNumber = transaction.Account?.AccountNumber,
                BillerId = transaction.BillerId,
                BillerName = transaction.Biller?.Name,
                BillerCode = transaction.Biller?.Code,
                ProductId = transaction.ProductId,
                ProductName = transaction.Product?.Name,
                ProductCategory = transaction.Product != null ? transaction.Product.Category.ToString() : null,
                ProductItemId = transaction.ProductItemId,
                ProductItemName = transaction.ProductItem?.Name,
                Amount = transaction.Amount,
                Status = transaction.Status,
                Reference = transaction.Reference,
                Description = transaction.Narration,
                CreatedAt = transaction.CreatedAt,
                CompletedAt = transaction.CompletedAt
            };
        }

        private static string GenerateReference()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        private static ApiResponse<TransactionResponseDto> Fail(string message, string code)
        {
            return ApiResponse<TransactionResponseDto>.Fail(message, new List<ApiError> { new(code, message) });
        }

        private static FieldValidationResult ValidateRequiredFields(E.Product product, Dictionary<string, object?> fields)
        {
            if (string.IsNullOrWhiteSpace(product.RequiredFields))
            {
                return FieldValidationResult.Valid();
            }

            var requiredFields = JsonSerializer.Deserialize<TatumConnectBackened.DTOs.ProductRequiredFieldsDto>(product.RequiredFields, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (requiredFields == null || requiredFields.Fields == null)
            {
                return FieldValidationResult.Valid();
            }

            var definitions = requiredFields.Fields;
            var errors = new List<ApiError>();

            foreach (var definition in definitions)
            {
                fields.TryGetValue(definition.Name, out var value);

                if (definition.Required && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
                {
                    errors.Add(new ApiError("RequiredField", $"{definition.Label} is required."));
                    continue;
                }

                if (value == null)
                    continue;

                if (definition.AllowedValues != null && definition.AllowedValues.Count > 0)
                {
                    if (!definition.AllowedValues.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase))
                    {
                        errors.Add(new ApiError("InvalidFieldValue", $"{definition.Label} contains an invalid value."));
                    }
                }

                if (!string.IsNullOrWhiteSpace(definition.Validation?.Pattern))
                {
                    var input = value.ToString() ?? string.Empty;
                    if (!Regex.IsMatch(input, definition.Validation.Pattern))
                    {
                        errors.Add(new ApiError("InvalidFieldFormat", $"{definition.Label} has an invalid format."));
                    }
                }
            }

            if (errors.Count > 0)
            {
                return new FieldValidationResult { Success = false, Message = "One or more required fields are invalid.", Errors = errors };
            }

            return FieldValidationResult.Valid();
        }

        private static (DateTime From, DateTime To) ResolveDateRange(TransactionSummaryFilterDto filter)
        {
            if (filter.FromDate.HasValue || filter.ToDate.HasValue)
            {
                var from = filter.FromDate?.Date ?? DateTime.UtcNow.Date;
                var to = filter.ToDate?.Date.AddDays(1) ?? DateTime.UtcNow.Date.AddDays(1);
                if (from >= to) throw new ArgumentException("FromDate must be earlier than ToDate.");
                return (from, to);
            }

            var today = DateTime.UtcNow.Date;
            return filter.Period switch
            {
                TransactionPeriod.Yesterday => (today.AddDays(-1), today),
                TransactionPeriod.Last7Days => (today.AddDays(-7), today.AddDays(1)),
                TransactionPeriod.Last30Days => (today.AddDays(-30), today.AddDays(1)),
                TransactionPeriod.Last90Days => (today.AddDays(-90), today.AddDays(1)),
                _ => (today.AddDays(-30), today.AddDays(1))
            };
        }

        private static bool IsValidNigerianPhone(string phone)
        {
            if (phone.StartsWith("+234")) phone = "0" + phone.Substring(4);
            return phone.Length == 11 && phone.StartsWith("0") && phone.All(char.IsDigit);
        }

        private class FieldValidationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<ApiError> Errors { get; set; } = new();
            public static FieldValidationResult Valid() => new FieldValidationResult { Success = true };
        }
    }
}
