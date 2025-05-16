using Microsoft.EntityFrameworkCore;
using MovieRecommender.Entities;

namespace MovieRecommender.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }

    public DbSet<WatchlistEntryEntity> WatchlistEntries { get; set; }

    public DbSet<RecommendationParamsEntity> RecommendationParams { get; set; }

    public DbSet<RecommendationEntity> Recommendations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<WatchlistEntryEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecommendationParamsEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasOne<UserEntity>()
                .WithOne()
                .HasForeignKey<RecommendationParamsEntity>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecommendationEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasOne<UserEntity>()
                .WithOne()
                .HasForeignKey<RecommendationEntity>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}