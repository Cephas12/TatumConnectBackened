namespace TatumConnectBackened.DTOs
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public decimal AvailableBalance { get; set; }
        public decimal LedgerBalance { get; set; }
        public string Status { get; set; } = null!;
    }
}
