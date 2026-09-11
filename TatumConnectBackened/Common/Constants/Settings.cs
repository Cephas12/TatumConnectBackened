namespace TatumConnectBackened.Common.Constants
{
    public class SmsSettings
    {
        public string Provider { get; set; } = "Termil";
        public string ApiKey { get; set; } = null!;
        public string SenderId { get; set; } = "TatumConnect";
        public string BaseURl { get; set; } = "https://api.ng/termii.com";
    }
}
