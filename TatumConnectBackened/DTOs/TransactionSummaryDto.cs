namespace TatumConnectBackened.DTOs
{
    public class TransactionSummaryDto
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int TotalTransactions { get; set; }

        public int SuccessfulTransactions { get; set; }

        public int FailedTransactions { get; set; }

        public int PendingTransactions { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal SuccessfulAmount { get; set; }

        public decimal FailedAmount { get; set; }

        public decimal PendingAmount { get; set; }

        public PercentageChangeDto
            TransactionCountChange
        { get; set; }
            = new();

        public PercentageChangeDto
            TransactionAmountChange
        { get; set; }
            = new();

        public List<TransactionCategorySummaryDto>
            ByCategory
        { get; set; } = new();

        public List<TransactionBillerSummaryDto>
            ByBiller
        { get; set; } = new();
    }

    public class PercentageChangeDto
    {
        public decimal Current { get; set; }

        public decimal Previous { get; set; }

        public decimal Percentage { get; set; }
    }

    public class TransactionResponseDto
    {
        public Guid Id { get; set; }

        public string Reference { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Status { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = null!;

        public string? BillerName { get; set; }

        public string? ProductName { get; set; }

        public string? ProductItemName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }

    public class TransactionCategorySummaryDto
    {
        public string Category { get; set; } = string.Empty;

        public int Count { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PercentageOfTransactions { get; set; }
    }

    public class TransactionBillerSummaryDto
    {
        public Guid? BillerId { get; set; }

        public string BillerName { get; set; } = string.Empty;

        public string BillerCode { get; set; } = string.Empty;

        public int Count { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PercentageOfTransactions { get; set; }
    }
}
