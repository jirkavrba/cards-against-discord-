using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardsAgainstDiscord.Migrations
{
    public partial class JoiningLeavingPlayers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_picks_games_game_id",
                table: "picks");

            migrationBuilder.DropIndex(
                name: "ix_picks_game_id",
                table: "picks");

            migrationBuilder.DropColumn(
                name: "game_id",
                table: "picks");

            migrationBuilder.AddColumn<decimal[]>(
                name: "joining_players",
                table: "games",
                type: "numeric(20,0)[]",
                nullable: false,
                defaultValue: new decimal[0]);

            migrationBuilder.AddColumn<decimal[]>(
                name: "leaving_players",
                table: "games",
                type: "numeric(20,0)[]",
                nullable: false,
                defaultValue: new decimal[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "joining_players",
                table: "games");

            migrationBuilder.DropColumn(
                name: "leaving_players",
                table: "games");

            migrationBuilder.AddColumn<int>(
                name: "game_id",
                table: "picks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_picks_game_id",
                table: "picks",
                column: "game_id");

            migrationBuilder.AddForeignKey(
                name: "fk_picks_games_game_id",
                table: "picks",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id");
        }
    }
}
