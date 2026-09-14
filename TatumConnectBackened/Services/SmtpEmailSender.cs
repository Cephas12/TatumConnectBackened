using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;
using MimeKit;
//using System.Net.Mail;
using TatumConnectBackened.Auth;

namespace TatumConnectBackened.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailSettings> settings, ILogger<SmtpEmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }
        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var smtp = _settings?.Smtp;
            if (smtp == null || string.IsNullOrWhiteSpace(smtp.Host))
            {
                _logger.LogWarning("SMTP host is not configured. Email to {Email} skipped.", to);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(
                    _settings.FromName ?? "TatumConnect",
                    _settings.FromEmail ?? "noreply@tatumconnect.com"));
            message.To.Add(
                MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();
            using var client = new SmtpClient();
            try
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

                await client.ConnectAsync(
                    smtp.Host,
                    smtp.Port,
                    SecureSocketOptions.SslOnConnect,
                    linkedCts.Token);
                await client.AuthenticateAsync(
                    smtp.Username,
                    smtp.Password, linkedCts.Token);
                await client.SendAsync(message, linkedCts.Token);
                _logger.LogInformation("Email successfully sent to {Email}", to);

            }
            catch(OperationCanceledException)
            {
                _logger.LogWarning("Email sending timed out or was cancelled for {Email}", to);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
            }
        }
    }
}
