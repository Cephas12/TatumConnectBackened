using System.ComponentModel.DataAnnotations.Schema;

namespace TatumConnectBackened.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        public User? User { get; set; }

        public Guid AccountId { get; set; }
        public Account? Account { get; set; }

        public Guid? BillerId { get; set; }
        public Biller? Biller { get; set; }

        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        public Guid? ProductItemId { get; set; }
        public ProductItem? ProductItem { get; set; }

        public string Reference { get; set; } = string.Empty;

        public string? Type { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "NGN";

        public string? Narration { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }
    }
}
