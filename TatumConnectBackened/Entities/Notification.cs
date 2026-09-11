namespace TatumConnectBackened.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "General";
        public string Status { get; set; } = "Unread";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt {  get; set; }
    }
}
