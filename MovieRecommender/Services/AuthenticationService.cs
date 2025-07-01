using Microsoft.EntityFrameworkCore;
using MovieRecommender.Contexts;
using MovieRecommender.Entities;

namespace MovieRecommender.Services;

public class AuthenticationService(ApplicationDbContext context)
{
    public async Task<UserEntity> GetUserFromRequest(HttpRequest request)
    {
        var authHeader = request.Headers.Authorization;
        var authString = authHeader.ToString();
        if (string.IsNullOrEmpty(authString) || !authString.StartsWith("Bearer "))
        {
            throw new UnauthorizedAccessException("Invalid or missing token");
        }

        var token = authString["Bearer ".Length..].Trim();

        return await GetFromToken(token);
    }

    public async Task<UserEntity> GetFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        }

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Token == token);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        return user;
    }

    public async Task<UserEntity?> GetFromTokenOptional(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        return await context.Users
            .FirstOrDefaultAsync(u => u.Token == token);
    }

    public async Task<bool> IsValidToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Token == token);

        return user != null;
    }
}