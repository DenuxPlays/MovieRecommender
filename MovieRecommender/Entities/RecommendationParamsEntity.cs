using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieRecommender.Entities;

public class RecommendationParamsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    public required int UserId { get; init; }

    public required int MostWatchedGenre { get; set; }

    public required int[] RecommendedGenres { get; set; }

    public required int[] RecommendedKeywords { get; set; }
}