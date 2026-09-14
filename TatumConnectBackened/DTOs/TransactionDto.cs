// Remove unused System.Transactions import which causes ambiguity for 'Transaction' types
// The project has its own Entities.Transaction and DTOs.Transaction types.
using TatumConnectBackened.Common.Constants;
// Note: DTOs should reference common constants for enums such as TransactionStatus

namespace TatumConnectBackened.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string? UserName { get; set; }

        public Guid AccountId { get; set; }

        public string? AccountNumber { get; set; }

        public Guid? BillerId { get; set; }

        public string? BillerName { get; set; }

        public string? BillerCode { get; set; }

        public Guid? ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductCategory { get; set; }

        public Guid? ProductItemId { get; set; }

        public string? ProductItemName { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Reference { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }

    public class TransactionFilterDto
    {
        public Guid? TransactionId { get; set; }
        public Guid? AccountId { get; set; }

        public string? AccountNumber { get; set; }

        public Guid? UserId { get; set; }

        public Guid? BillerId { get; set; }

        public Guid? ProductId { get; set; }

        public TatumConnectBackened.Common.Constants.TransactionStatus? Status { get; set; }
        // DTO placeholder for product category; use string to avoid duplicate enum definitions
        public string? Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }

    public class TransactionSummaryFilterDto
    {
        public TransactionPeriod? Period { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
