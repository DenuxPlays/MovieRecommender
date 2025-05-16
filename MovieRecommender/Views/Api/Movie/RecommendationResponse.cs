namespace MovieRecommender.Views.Api.Movie;

public class RecommendationResponse(int[] globalRecommendedMovieIds, int[] genreRecommendedMovieIds)
{
    public int[] GlobalRecommendedMovieIds { get; set; } = globalRecommendedMovieIds;
    public int[] GenreRecommendedMovieIds { get; set; } = genreRecommendedMovieIds;
}