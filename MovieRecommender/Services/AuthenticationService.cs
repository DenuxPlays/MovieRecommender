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
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Invalid or missing token");
        }

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Token == token);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        return user;
    }
}