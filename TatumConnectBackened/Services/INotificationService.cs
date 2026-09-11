using TatumConnectBackened.DTOs;
using TatumConnectBackened.Responses;

namespace TatumConnectBackened.Services
{
    public interface INotificationService
    {
        Task<ApiResponse<GCNotificationStatus>> SendAsync(
            Guid customerId,
            string subject,
            string message,
            string type = "General",
            CancellationToken ct = default);

        Task<ApiResponse<object>> SendAsync(
            Guid customerId,
            string subject,
            string message);

        Task<ApiResponse<object>> SendEmailAsync(
            string email,
            string subject,
            string htmlMessage,
            CancellationToken ct = default);

        Task<ApiResponse<object>> SendSmsAsync(
            string phone,
            string message,
            CancellationToken ct = default);

        Task<ApiResponse<List<NotificationDto>>>
            GetByCustomerAsync(
            Guid customerId,
            CancellationToken ct = default);
    }
}
