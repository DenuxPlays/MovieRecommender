using MovieRecommender.Views.Api.Movie;
using TMDbLib.Client;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.Movies;

namespace MovieRecommender.Services;

public class TmdbService(TMDbClient tmDbClient)
{
    public List<MovieResponse> Search(string query)
    {
        var search = tmDbClient.SearchMovieAsync(query).Result;
        var movies = search.Results.Select(m => new MovieResponse(
            m.Id,
            m.Title,
            m.Overview,
            m.ReleaseDate,
            m.PosterPath,
            m.GenreIds,
            BuildImageUrl(m.PosterPath)
        )).ToList();

        return movies;
    }

    public async Task<MovieResponse?> GetMovie(int id)
    {
        var movie = await tmDbClient.GetMovieAsync(id);
        if (movie == null)
        {
            return null;
        }

        return new MovieResponse(
            movie.Id,
            movie.Title,
            movie.Overview,
            movie.ReleaseDate,
            movie.PosterPath,
            movie.Genres.Select(g => g.Id).ToList(),
            BuildImageUrl(movie.PosterPath)
        );
    }

    public async Task<KeywordsContainer> GetKeywordsForMovie(int moveId)
    {
        return await tmDbClient.GetMovieKeywordsAsync(moveId);
    }


    public async Task<List<MovieResponse>> GetGlobalRecommendations(int[] genreIds, int[] keywordIds,
        int[] excludedMovies, int page = 1)
    {
        var discoverQuery = tmDbClient.DiscoverMoviesAsync();
        if (discoverQuery == null)
        {
            throw new InvalidOperationException("Failed to create discover query.");
        }

        discoverQuery.IncludeWithAnyOfGenre(genreIds);
        discoverQuery.IncludeWithAnyOfKeywords(keywordIds);
        discoverQuery.OrderBy(DiscoverMovieSortBy.PopularityDesc);

        var result = await discoverQuery.Query(page);

        var filteredResults = result.Results
            .Where(m => !excludedMovies.Contains(m.Id))
            .ToList();

        return filteredResults.Select(m => new MovieResponse(
            m.Id,
            m.Title,
            m.Overview,
            m.ReleaseDate,
            m.PosterPath,
            m.GenreIds,
            BuildImageUrl(m.PosterPath)
        )).ToList();
    }

    public async Task<List<MovieResponse>> GetMoviesByGenre(int genreId, int[] excludedMovies, int page = 1)
    {
        var discoverQuery = tmDbClient.DiscoverMoviesAsync();
        if (discoverQuery == null)
        {
            throw new InvalidOperationException("Failed to create discover query.");
        }

        discoverQuery.IncludeWithAllOfGenre([genreId]);
        discoverQuery.OrderBy(DiscoverMovieSortBy.PopularityDesc);

        var result = await discoverQuery.Query(page);

        var filteredResults = result.Results
            .Where(m => !excludedMovies.Contains(m.Id))
            .ToList();

        return filteredResults.Select(m => new MovieResponse(
            m.Id,
            m.Title,
            m.Overview,
            m.ReleaseDate,
            m.PosterPath,
            m.GenreIds,
            BuildImageUrl(m.PosterPath)
        )).ToList();
    }

    private static string BuildImageUrl(string path, string size = "w500")
    {
        return string.IsNullOrEmpty(path) ? string.Empty : $"https://image.tmdb.org/t/p/{size}/{path}";
    }
}