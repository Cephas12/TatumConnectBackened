namespace TatumConnectBackened.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime? ReadAt {  get; set; }
    }
}
