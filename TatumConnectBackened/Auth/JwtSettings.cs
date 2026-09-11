namespace TatumConnectBackened.Auth
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }


        public string Secret { get; set; }

        public int AccessTokenMinutes { get; set; }

        public int RefreshTokenDays { get; set; }
    }
}
