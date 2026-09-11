using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TatumConnectBackened.Auth;
using TatumConnectBackened.Entities;



namespace TatumConnectBackened.Services
{
    public class JwtService
    {
        private readonly JwtSettings _settings;


        public JwtService(IOptions<JwtSettings> options)

        {

            _settings = options.Value;


            if (string.IsNullOrWhiteSpace(_settings.Secret))

            {

                throw new InvalidOperationException("JWT secret is not configured.");

            }

        }


        public string GenerateAccessToken(User user)

        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));


            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>

{

// Standard JET subject

new(

JwtRegisteredClaimNames.Sub,

user.Id.ToString()

),


// Email

new(

JwtRegisteredClaimNames.Email,

user.Email

),


// ASP.NT Core user ID

new(

ClaimTypes.NameIdentifier,

user.Id.ToString()

),


// ASP.NET Core Email

new(

ClaimTypes.Email,

user.Email

),


// ASP.NET Core Role

new(

ClaimTypes.Role,

user.Role)

};


            var token = new JwtSecurityToken(

            issuer: _settings.Issuer,

            audience: _settings.Audience,

            claims: claims,

            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),

            signingCredentials: credentials

            );


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    }
}
