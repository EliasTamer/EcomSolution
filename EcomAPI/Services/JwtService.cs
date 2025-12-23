using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcomAPI.Entities;
using EcomAPI.Interfaces;

namespace EcomAPI.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService (IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            var secret = _configuration["JwtSettings:Secret"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpiryInMinutes"]);

            // transform the secret into a an array of bytes
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            // state the algo that will be used to sign the token
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // claims are info related to the user
            var claims = new[]
            {
                new Claim("Email", user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Role", user.Role)
            };

            // create the token 
            var token = new JwtSecurityToken(    
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            // seriealize token to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
