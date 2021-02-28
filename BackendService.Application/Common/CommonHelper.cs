
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendService.Application.Settings;
using BackendService.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BackendService.Application.Common
{
    public class CommonHelper : ICommonHelper
    {
        private readonly JwtSettings _jwtSettings;

        public CommonHelper(IOptions<JwtSettings> appSettings)
        {
            _jwtSettings = appSettings.Value;
        }

        public string GenerateJwtToken(string id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, id)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public string GenerateCode()
        {
            Random generator = new Random();
            String randomCode = generator.Next(0, 999999).ToString("D6");
            return randomCode;
        }
        public string SetSecretEmail(string email)
        {
            var splits = email.Split("@");
            return splits[0][0] + "***@" + splits[1];
        }
    }
}