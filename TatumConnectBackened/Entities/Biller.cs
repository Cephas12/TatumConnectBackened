    namespace TatumConnectBackened.Entities
{
    public class Biller
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Internal unique code.
        /// Example: MTN, AIRTEL, DSTV.
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Display name.
        /// Example: MTN Nigeria.
        /// </summary>
        public string Name { get; set; } = null!;

        public BillerCategory Category { get; set; }

        /// <summary>
        /// Optional logo URL.
        /// </summary>
        public string? LogoUrl { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
