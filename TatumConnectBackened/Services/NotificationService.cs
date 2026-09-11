using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TatumConnectBackened.Responses;
using System.Text;
using System.Text.Json;
using TatumConnectBackened.DTOs;
using TatumConnectBackened.Repositories;
using TatumConnectBackened.Common.Constants;
using System.Web.Helpers;
//using System.Web.Helpers;






namespace TatumConnectBackened.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository
            _notificaitonRepository;
        private readonly IEmailSender _emailSender;
        private readonly HttpClient _httpClient;
        private readonly SmsSettings _smsSettings;
        private readonly string json;

        public NotificationService(
            INotificationRepository notificationRepository, IEmailSender emailSender, IOptions<SmsSettings> smsOptions, HttpClient httpClient)
        {
            _notificaitonRepository = notificationRepository;
            _emailSender = emailSender;
            _smsSettings = smsOptions.Value;
            _httpClient = httpClient;
        }

        public Task<ApiResponse<List<NotificationDto>>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<GCNotificationStatus>> SendAsync(Guid customerId, string subject, string message, string type = "General", CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<object>> SendAsync(Guid customerId, string subject, string message)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<object>> SendEmailAsync(
            string email,
            string subject,
            string htmlMessage,
            CancellationToken ct = default)
        {
            try
            {
                await _emailSender.SendAsync(
                    email,
                    subject,
                    htmlMessage,
                    ct);
                return ApiResponse<object>.Ok(
                    new
                    {
                        sendt = true,
                    },
                    "Email sent successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail(
                    "Failed to send email.",
                    new List<ApiError>
                    {
                        new(
                            "EmailSendFailed",
                            ex.Message)
                    });
            }


        }
        public async Task<ApiResponse<object>> SendSmsAsync(string phoneNumber, string message, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    return ApiResponse<object>.Fail("Phone number is required.", new List<ApiError> { new("InvalidPhoneNumber", "A valid phone number is required.") });
                }
                var payload = new { api_key = _smsSettings.ApiKey, to = phoneNumber, from = _smsSettings.SenderId, sms = message, type = "plain", channel = "generic" };
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_smsSettings.BaseURl}/api/sms/send", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return ApiResponse<object>.Fail("Failed to send SMS.", new List<ApiError> { new("SmsSendFailed", error) });
                }
                return ApiResponse<object>.Ok((object?)"SMS sent successfully.");

            }
            catch(Exception ex)
            {
                return ApiResponse<object>.Fail("An error occured while sending the SMS", new List<ApiError> { new("SmsException", ex.Message) });
            }



        }
    }
}
