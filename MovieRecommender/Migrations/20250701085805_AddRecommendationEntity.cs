using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieRecommender.Migrations
{
    /// <inheritdoc />
    public partial class AddRecommendationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecommendationParamsEntity_Users_UserId",
                table: "RecommendationParamsEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecommendationParamsEntity",
                table: "RecommendationParamsEntity");

            migrationBuilder.RenameTable(
                name: "RecommendationParamsEntity",
                newName: "RecommendationParams");

            migrationBuilder.RenameIndex(
                name: "IX_RecommendationParamsEntity_UserId",
                table: "RecommendationParams",
                newName: "IX_RecommendationParams_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecommendationParams",
                table: "RecommendationParams",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    globalRecommendedMovieIds = table.Column<string>(type: "TEXT", nullable: false),
                    genreRecommendedMovieIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recommendations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_UserId",
                table: "Recommendations",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecommendationParams_Users_UserId",
                table: "RecommendationParams",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecommendationParams_Users_UserId",
                table: "RecommendationParams");

            migrationBuilder.DropTable(
                name: "Recommendations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecommendationParams",
                table: "RecommendationParams");

            migrationBuilder.RenameTable(
                name: "RecommendationParams",
                newName: "RecommendationParamsEntity");

            migrationBuilder.RenameIndex(
                name: "IX_RecommendationParams_UserId",
                table: "RecommendationParamsEntity",
                newName: "IX_RecommendationParamsEntity_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecommendationParamsEntity",
                table: "RecommendationParamsEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecommendationParamsEntity_Users_UserId",
                table: "RecommendationParamsEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
