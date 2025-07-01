using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieRecommender.Contexts;
using MovieRecommender.Services;

namespace MovieRecommender.Controllers.Api;

[ApiController]
[Route("/api/watchlist")]
public class WatchlistController(
    ApplicationDbContext context,
    TmdbService tmdbService,
    AuthenticationService authenticationService,
    RecommendationService recommendationService,
    WatchlistService watchlistService
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

        await watchlistService.AddToWatchlist(user.Id, request.MovieId);

        return Ok("Movie added to watchlist");
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> RemoveFromWatchlist([FromBody] WatchlistRequest request)
    {
        var user = await authenticationService.GetUserFromRequest(Request);

        await watchlistService.RemoveFromWatchlist(user.Id, request.MovieId);

        return Ok("Movie removed from watchlist");
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<int[]>> GetWatchlist()
    {
        var user = await authenticationService.GetUserFromRequest(Request);

        var watchlist = await context.WatchlistEntries
            .Where(e => e.UserId == user.Id)
            .Select(e => e.MovieId)
            .ToArrayAsync();

        return Ok(watchlist);
    }
}

public class WatchlistRequest
{
    [JsonPropertyName("movieId")] public int MovieId { get; set; }
}