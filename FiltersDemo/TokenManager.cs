using FiltersDemo.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FiltersDemo
{
    public class TokenManager
    {
        private readonly IConfiguration _configuration;

        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string tokenGenerator(User user)
        {
            // JWT Token Generator
            byte[] keyArray = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]);
            var secretkey = new SymmetricSecurityKey(keyArray);
            var Credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

            var getClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.userName),
                    new Claim("password", user.Password),
                    new Claim(ClaimTypes.Role, user.Role)
                };

            var tokenOptions = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:audience"],
                    claims: getClaims,
                    signingCredentials: Credentials,
                    expires: DateTime.Now.AddMinutes(10)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }

        public bool validateToken(string token)
        {
            var keyArray = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]);
            var secretkey = new SymmetricSecurityKey(keyArray);

            var tokenValidation = new TokenValidationParameters()
            {
                IssuerSigningKey = secretkey,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                SecurityToken securityToken;
                tokenHandler.ValidateToken(token, tokenValidation, out securityToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
