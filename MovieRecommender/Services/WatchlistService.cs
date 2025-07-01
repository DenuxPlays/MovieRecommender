using Microsoft.EntityFrameworkCore;
using MovieRecommender.Contexts;
using MovieRecommender.Entities;
using MovieRecommender.Views.Api.Movie;

namespace MovieRecommender.Services;

public class WatchlistService(
    ApplicationDbContext context,
    TmdbService tmdbService,
    RecommendationService recommendationService)
{
    public async Task AddToWatchlist(int userId, int movieId)
    {
        var entry = await context.WatchlistEntries
            .FirstOrDefaultAsync(e => e.UserId == userId && e.MovieId == movieId);

        if (entry != null) return; // Movie already in watchlist

        var newEntry = new WatchlistEntryEntity
        {
            UserId = userId,
            MovieId = movieId
        };

        context.WatchlistEntries.Add(newEntry);
        await context.SaveChangesAsync();

        recommendationService.RunRecommendationProcess(userId);
    }

    public async Task<List<MovieResponse>> GetRenderableWatchlist(int userId)
    {
        var watchlist = await context.WatchlistEntries
            .Where(e => e.UserId == userId)
            .Select(e => e.MovieId)
            .ToArrayAsync();

        var movieResponseTasks = watchlist.Select(tmdbService.GetMovie);
        var movieResponses = await Task.WhenAll(movieResponseTasks);
        movieResponses = movieResponses.Where(m => m != null).ToArray();

        return movieResponses.ToList()!;
    }

    public async Task RemoveFromWatchlist(int userId, int movieId)
    {
        var entry = await context.WatchlistEntries
            .FirstOrDefaultAsync(e => e.UserId == userId && e.MovieId == movieId);

        if (entry != null)
        {
            context.WatchlistEntries.Remove(entry);
            await context.SaveChangesAsync();

            recommendationService.RunRecommendationProcess(userId);
        }
    }
}