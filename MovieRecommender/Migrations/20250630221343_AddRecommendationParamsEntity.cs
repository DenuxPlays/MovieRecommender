using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieRecommender.Migrations
{
    /// <inheritdoc />
    public partial class AddRecommendationParamsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecommendationParamsEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    MostWatchedGenre = table.Column<int>(type: "INTEGER", nullable: false),
                    RecommendedGenres = table.Column<string>(type: "TEXT", nullable: false),
                    RecommendedKeywords = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationParamsEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecommendationParamsEntity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationParamsEntity_UserId",
                table: "RecommendationParamsEntity",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecommendationParamsEntity");
        }
    }
}
