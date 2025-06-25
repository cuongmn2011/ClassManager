// File: src/Infrastructure/Services/TokenService.cs
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data; 
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq; 
using System.Security.Claims;
using System.Security.Cryptography; 
using System.Text;
using System.Threading.Tasks; 

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public TokenService(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
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
        public async Task<string> CreateAndSaveRefreshTokenAsync(User user)
        {
            // Generate a secure random string for the refresh token
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshTokenString = Convert.ToBase64String(randomNumber);

            // Create the refresh token entity
            var refreshToken = new UserRefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token expires in 7 days
                CreatedAt = DateTime.UtcNow
            };

            // Save the refresh token to the database
            await _context.UserRefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshTokenString;
        }
    }
}