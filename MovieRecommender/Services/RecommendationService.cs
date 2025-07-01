using MovieRecommender.Contexts;
using MovieRecommender.Entities;
using MovieRecommender.Views.Api.Movie;

namespace MovieRecommender.Services;

public class RecommendationService(
    ApplicationDbContext context,
    TmdbService tmdbService,
    IServiceProvider serviceProvider)
{
    private const int MaxMostWatchedGenres = 4;
    private const int MaxMostWatchedKeywords = 3;

    public RecommendationResponse GetRecommendations(int userId)
    {
        var recommendations = context.Recommendations
            .FirstOrDefault(r => r.UserId == userId);

        if (recommendations == null)
        {
            return new RecommendationResponse(
                [],
                []);
        }

        return new RecommendationResponse(
            recommendations.globalRecommendedMovieIds,
            recommendations.genreRecommendedMovieIds
        );
    }

    public void RunRecommendationProcess(int userId)
    {
        Task.Run(() =>
        {
            using var scope = serviceProvider.CreateScope();
            var scopedContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var scopedTmdbService = scope.ServiceProvider.GetRequiredService<TmdbService>();

            var service = new RecommendationService(scopedContext, scopedTmdbService, serviceProvider);
            service.ProcessRecommendationParams(userId);
            service.ProcessRecommendations(userId);
        });
    }

    private void ProcessRecommendations(int userId, int recursion = 0)
    {
        var recommendationParams = context.RecommendationParams
            .FirstOrDefault(p => p.UserId == userId);
        if (recommendationParams == null)
        {
            if (recursion > 3)
            {
                throw new InvalidOperationException("Failed to process recommendations after multiple attempts.");
            }

            ProcessRecommendationParams(userId);
            ProcessRecommendations(userId, recursion + 1);
        }

        var watchedMovies = context.WatchlistEntries
            .Where(e => e.UserId == userId)
            .Select(e => e.MovieId)
            .ToArray();

        var globalRecommendations = tmdbService.GetGlobalRecommendations(
            recommendationParams!.RecommendedGenres,
            recommendationParams.RecommendedKeywords,
            watchedMovies
        ).Result;

        var genreRecommendations = tmdbService.GetMoviesByGenre(
            recommendationParams.MostWatchedGenre, watchedMovies).Result;

        // Check if recommendations already exist
        var existingRecommendations = context.Recommendations
            .FirstOrDefault(r => r.UserId == userId);

        if (existingRecommendations != null)
        {
            // Update existing recommendations
            existingRecommendations.globalRecommendedMovieIds = globalRecommendations.Select(r => r.Id).ToArray();
            existingRecommendations.genreRecommendedMovieIds = genreRecommendations.Select(r => r.Id).ToArray();
            context.Recommendations.Update(existingRecommendations);
        }
        else
        {
            // Create new recommendations
            var recommendations = new RecommendationEntity()
            {
                globalRecommendedMovieIds = globalRecommendations.Select(r => r.Id).ToArray(),
                genreRecommendedMovieIds = genreRecommendations.Select(r => r.Id).ToArray(),
                UserId = userId
            };
            context.Recommendations.Add(recommendations);
        }

        context.SaveChanges();
    }

    private void ProcessRecommendationParams(int userId)
    {
        var user = context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var watchlistEntries = context.WatchlistEntries
            .Where(e => e.UserId == userId)
            .ToList();

        var mostWatchedGenres = FindMostWatchedGenres(watchlistEntries).Result;

        var genre = mostWatchedGenres.First();

        var recommendationGenres = mostWatchedGenres
            .Skip(1);

        var keywords = FindMostWatchedKeywords(watchlistEntries).Result;

        // Check if recommendation params already exist
        var existingParams = context.RecommendationParams
            .FirstOrDefault(p => p.UserId == userId);

        if (existingParams != null)
        {
            // Update existing params
            existingParams.MostWatchedGenre = genre;
            existingParams.RecommendedGenres = recommendationGenres.ToArray();
            existingParams.RecommendedKeywords = keywords.ToArray();
            context.RecommendationParams.Update(existingParams);
        }
        else
        {
            // Create new params
            var recommendationParams = new RecommendationParamsEntity()
            {
                UserId = userId,
                MostWatchedGenre = genre,
                RecommendedGenres = recommendationGenres.ToArray(),
                RecommendedKeywords = keywords.ToArray()
            };
            context.RecommendationParams.Add(recommendationParams);
        }

        context.SaveChanges();
    }

    private async Task<List<int>> FindMostWatchedGenres(List<WatchlistEntryEntity> watchlistEntries)
    {
        if (watchlistEntries.Count == 0)
        {
            return [];
        }

        var movies = await Task.WhenAll(watchlistEntries.Select(e => tmdbService.GetMovie(e.MovieId)));

        return movies
            .Where(movie => movie != null)
            .SelectMany(movie => movie!.GenreIds)
            .GroupBy(genreId => genreId)
            .OrderByDescending(group => group.Count())
            .Take(MaxMostWatchedGenres)
            .Select(group => group.Key)
            .ToList();
    }

    private async Task<List<int>> FindMostWatchedKeywords(List<WatchlistEntryEntity> watchlistEntries)
    {
        if (watchlistEntries.Count == 0)
        {
            return [];
        }

        var keywords = await Task.WhenAll(watchlistEntries.Select(e => tmdbService.GetKeywordsForMovie(e.MovieId)));

        return keywords
            .SelectMany(keyword => keyword.Keywords.Select(k => k.Id))
            .GroupBy(keywordId => keywordId)
            .OrderByDescending(group => group.Count())
            .Take(MaxMostWatchedKeywords)
            .Select(group => group.Key)
            .ToList();
    }
}