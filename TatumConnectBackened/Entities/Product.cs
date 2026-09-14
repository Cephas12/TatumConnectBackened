namespace TatumConnectBackened.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public Guid BillerId { get; set; }

        public Biller Biller { get; set; } = null!;

        /// <summary>
        /// Internal product code.
        /// Example: AIRTIME, DATA.
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Display name.
        /// Example: Airtime, Data Bundle.
        /// </summary>
        public string Name { get; set; } = null!;

        public ProductCategory Category { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// Product-level unit price.
        ///
        /// Useful for products such as Airtime where
        /// the customer chooses the amount dynamically.
        ///
        /// Nullable because products with ProductItems
        /// may have their prices defined by the ProductItem.
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// Indicates whether the customer can enter
        /// a custom amount.
        ///
        /// Example:
        /// Airtime = true
        /// Data Bundle = false
        /// </summary>
        public bool AllowsCustomAmount { get; set; }

        public bool RequiresProductItem { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// JSON describing fields the frontend must collect.
        /// </summary>
        public string? RequiredFields { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductItem> ProductItems { get; set; }
            = new List<ProductItem>();
    }
}
