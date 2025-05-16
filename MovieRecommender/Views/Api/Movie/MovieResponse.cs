namespace MovieRecommender.Views.Api.Movie;

public class MovieResponse(
    int id,
    string title,
    string overview,
    DateTime? releaseDate,
    string posterPath,
    List<int> genreIds,
    string imageUrl)
{
    public int Id { get; set; } = id;

    public string Title { get; set; } = title;

    public string Overview { get; set; } = overview;

    public DateTime? ReleaseDate { get; set; } = releaseDate;

    public string PosterPath { get; set; } = posterPath;

    public List<int> GenreIds { get; set; } = genreIds;

    public string ImageUrl { get; set; } = imageUrl;
}