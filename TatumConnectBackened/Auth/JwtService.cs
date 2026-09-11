using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Auth
{
    public class JwtService
    {
        private readonly JwtSettings _settings;
        public JwtService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
            if (string.IsNullOrWhiteSpace(_settings.Secret))
            {
                throw new InvalidOperationException("JWT Secret is not configured.");
            }
        }
        public string GenerateAccessToken(
            User user)
        {
            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _settings.Secret));
            var credentialas =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);
            var claims =
                new List<Claim>
                {
                    //Standard JWT subject
                    new(
                        JwtRegisteredClaimNames.Sub,
                        user.Id.ToString()),
                    //Email

                    new(JwtRegisteredClaimNames.Email,
                    user.Email),

                    //ASP .NET Core user ID


                    new(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),
                    //ASP>NET Core email
                    new(
                        ClaimTypes.Email,
                        user.Email),
                    //ASP.NET Core role
                    new(
                        ClaimTypes.Role,
                        user.Role)
                };
            var token =
                new JwtSecurityToken(
                    issuer:
                       _settings.Issuer,
                    audience:
                    _settings.Audience,
                    claims:
                    claims,
                    expires:
                    DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
                    signingCredentials: credentialas);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
