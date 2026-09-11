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
            var smtp = _settings.Smtp;
            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(
                    _settings.FromName,
                    _settings.FromEmail));
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
                await client.ConnectAsync(
                    smtp.Host,
                    smtp.Port,
                    SecureSocketOptions.SslOnConnect,
                    ct);
                await client.AuthenticateAsync(
                    smtp.Username,
                 smtp.Password, ct);
                await client.SendAsync(message, ct);
                _logger.LogInformation("Email successfully sent to {Email}", to);

          }
            catch(OperationCanceledException)
            {
                _logger.LogWarning("Email sending was cancelled for {Email}", to);
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
            }
        }
    }
}
