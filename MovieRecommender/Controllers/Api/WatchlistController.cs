using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieRecommender.Contexts;
using MovieRecommender.Entities;
using MovieRecommender.Services;

namespace MovieRecommender.Controllers.Api;

[ApiController]
[Route("/api/watchlist")]
public class WatchlistController(
    ApplicationDbContext context,
    TmdbService tmdbService,
    AuthenticationService authenticationService,
    RecommendationService recommendationService
)
    : Controller
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddToWatchlist([FromBody] WatchlistRequest request)
    {
        var user = await authenticationService.GetUserFromRequest(Request);

        var movie = await tmdbService.GetMovie(request.MovieId);
        if (movie == null)
        {
            return NotFound("Movie not found");
        }

        var entry = await context.WatchlistEntries
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.MovieId == request.MovieId);

        if (entry != null) return Ok("Movie already in watchlist");

        var newEntry = new WatchlistEntryEntity
        {
            UserId = user.Id,
            MovieId = request.MovieId,
        };

        context.WatchlistEntries.Add(newEntry);
        await context.SaveChangesAsync();

        recommendationService.RunRecommendationProcess(user.Id);

        return Ok("Movie added to watchlist");
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> RemoveFromWatchlist([FromBody] WatchlistRequest request)
    {
        var user = await authenticationService.GetUserFromRequest(Request);

        var entry = await context.WatchlistEntries
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.MovieId == request.MovieId);

        if (entry == null) return NotFound("Movie not found in watchlist");

        context.WatchlistEntries.Remove(entry);
        await context.SaveChangesAsync();

        recommendationService.RunRecommendationProcess(user.Id);

        return Ok("Movie removed from watchlist");
    }
}

public class WatchlistRequest
{
    [JsonPropertyName("movieId")] public int MovieId { get; set; }
}