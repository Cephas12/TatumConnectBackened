namespace TatumConnectBackened.Auth
{
    public class EmailSettings
    {
        /// <summary>
        /// Email provider:
        /// "smtp" 
        /// </summary>
        public string Provider { get; set; } = "smtp";
        public string FromEmail { get; set; } = null!;
        /// <summary>
        /// Sender display name.
        /// </summary>

        public string FromName { get; set; } = "TatumConnect";
        /// <summary>
        /// Frontend URL for password setup.
        /// Example:
        /// https://app.tatumconnect.com
        /// </summary>
        public string FrontendBaseUrl { get; set; } = null!;
        public SmtpSettings Smtp { get; set; } = new();
        public SendGridSettings SendGrid { get; set; } = new();
    }
    public class SmtpSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EnableSsl { get; set; } = true;

    }
    public class SendGridSettings
    {
        public string ApiKey { get; set; } = null!;
        public string ApiUrl { get; set; } =
            "https://api.sendgrid.com/v3/mail/send";
    }
}

