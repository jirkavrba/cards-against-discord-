using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardsAgainstDiscord.Migrations
{
    public partial class RemoveObsoleteRoundId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_picks_games_game_id",
                table: "picks");

            migrationBuilder.DropColumn(
                name: "round_id",
                table: "picks");

            migrationBuilder.AlterColumn<int>(
                name: "game_id",
                table: "picks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_picks_games_game_id",
                table: "picks",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_picks_games_game_id",
                table: "picks");

            migrationBuilder.AlterColumn<int>(
                name: "game_id",
                table: "picks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "round_id",
                table: "picks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "fk_picks_games_game_id",
                table: "picks",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
