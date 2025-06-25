// File: src/Application/Interfaces/ITokenService.cs
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Creates a JWT for a given user and their roles.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <param name="roles">A list of roles the user belongs to.</param>
        /// <returns>A JWT string.</returns>
        string CreateToken(User user, IList<string> roles);
        Task<string> CreateAndSaveRefreshTokenAsync(User user);
    }
}