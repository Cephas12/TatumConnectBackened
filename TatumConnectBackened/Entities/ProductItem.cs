namespace TatumConnectBackened.Entities
{
    public class ProductItem
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;

        /// <summary>
        /// Internal code.
        /// Example: DATA_7GB_14DAYS.
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Display name.
        /// Example: 7GB - 14 Days.
        /// </summary>
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        /// <summary>
        /// Price for this specific item.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Optional quantity/value represented by the item.
        ///
        /// Example:
        /// 7
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Example:
        /// GB
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Example:
        /// 14 Days
        /// </summary>
        public int? ValidityDays { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Allows the item to override or extend
        /// the product's required fields.
        /// </summary>
        public string? RequiredFields { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}