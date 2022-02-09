using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardsAgainstDiscord.Migrations
{
    public partial class AddSelectedWinnerIdToGames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "selected_winner_id",
                table: "games",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selected_winner_id",
                table: "games");
        }
    }
}
