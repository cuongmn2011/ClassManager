// File: src/Infrastructure/Services/TokenService.cs
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // Get the secret key from configuration and create a SymmetricSecurityKey
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT_SECRET"]));
        }

        public string CreateToken(User user, IList<string> roles)
        {
            // Create a list of claims. Claims are statements about the user.
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create signing credentials using the secret key
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            // Describe the token that we are going to create
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time (e.g., 1 hour)
                Issuer = _config["JWT_ISSUER"],
                Audience = _config["JWT_AUDIENCE"],
                SigningCredentials = creds
            };

            // Create the token handler and the token itself
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Write the token to a string
            return tokenHandler.WriteToken(token);
        }
    }
}