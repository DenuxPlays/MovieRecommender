using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieRecommender.Entities;

public class WatchlistEntryEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    public required int UserId { get; init; }

    public required int MovieId { get; init; }
}
