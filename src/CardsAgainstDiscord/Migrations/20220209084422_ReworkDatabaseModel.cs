using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CardsAgainstDiscord.Migrations
{
    public partial class ReworkDatabaseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "black_cards",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false),
                    picks = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_black_cards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lobbies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    message_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    owner_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    joined_players = table.Column<decimal[]>(type: "numeric(20,0)[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lobbies", x => x.id);
                    table.UniqueConstraint("ak_lobbies_guild_id_channel_id_message_id", x => new { x.guild_id, x.channel_id, x.message_id });
                });

            migrationBuilder.CreateTable(
                name: "white_cards",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_white_cards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "black_card_game",
                columns: table => new
                {
                    games_id = table.Column<int>(type: "integer", nullable: false),
                    used_black_cards_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_black_card_game", x => new { x.games_id, x.used_black_cards_id });
                    table.ForeignKey(
                        name: "fk_black_card_game_black_cards_used_black_cards_id",
                        column: x => x.used_black_cards_id,
                        principalTable: "black_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_white_card",
                columns: table => new
                {
                    games_id = table.Column<int>(type: "integer", nullable: false),
                    used_white_cards_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_white_card", x => new { x.games_id, x.used_white_cards_id });
                    table.ForeignKey(
                        name: "fk_game_white_card_white_cards_used_white_cards_id",
                        column: x => x.used_white_cards_id,
                        principalTable: "white_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    message_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    black_card_id = table.Column<int>(type: "integer", nullable: true),
                    judge_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.id);
                    table.ForeignKey(
                        name: "fk_games_black_cards_black_card_id",
                        column: x => x.black_card_id,
                        principalTable: "black_cards",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    selected_white_card_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_players", x => x.id);
                    table.ForeignKey(
                        name: "fk_players_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_players_white_cards_selected_white_card_id",
                        column: x => x.selected_white_card_id,
                        principalTable: "white_cards",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "picks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    round_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    white_card_id = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_picks", x => x.id);
                    table.ForeignKey(
                        name: "fk_picks_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_picks_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_picks_white_cards_white_card_id",
                        column: x => x.white_card_id,
                        principalTable: "white_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_white_card",
                columns: table => new
                {
                    players_id = table.Column<int>(type: "integer", nullable: false),
                    white_cards_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_white_card", x => new { x.players_id, x.white_cards_id });
                    table.ForeignKey(
                        name: "fk_player_white_card_players_players_id",
                        column: x => x.players_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_white_card_white_cards_white_cards_id",
                        column: x => x.white_cards_id,
                        principalTable: "white_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_black_card_game_used_black_cards_id",
                table: "black_card_game",
                column: "used_black_cards_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_white_card_used_white_cards_id",
                table: "game_white_card",
                column: "used_white_cards_id");

            migrationBuilder.CreateIndex(
                name: "ix_games_black_card_id",
                table: "games",
                column: "black_card_id");

            migrationBuilder.CreateIndex(
                name: "ix_games_judge_id",
                table: "games",
                column: "judge_id");

            migrationBuilder.CreateIndex(
                name: "ix_picks_game_id",
                table: "picks",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_picks_player_id",
                table: "picks",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_picks_white_card_id",
                table: "picks",
                column: "white_card_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_white_card_white_cards_id",
                table: "player_white_card",
                column: "white_cards_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_game_id",
                table: "players",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_selected_white_card_id",
                table: "players",
                column: "selected_white_card_id");

            migrationBuilder.AddForeignKey(
                name: "fk_black_card_game_games_games_id",
                table: "black_card_game",
                column: "games_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_white_card_games_games_id",
                table: "game_white_card",
                column: "games_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_games_players_judge_id",
                table: "games",
                column: "judge_id",
                principalTable: "players",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_black_cards_black_card_id",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "fk_players_games_game_id",
                table: "players");

            migrationBuilder.DropTable(
                name: "black_card_game");

            migrationBuilder.DropTable(
                name: "game_white_card");

            migrationBuilder.DropTable(
                name: "lobbies");

            migrationBuilder.DropTable(
                name: "picks");

            migrationBuilder.DropTable(
                name: "player_white_card");

            migrationBuilder.DropTable(
                name: "black_cards");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "white_cards");
        }
    }
}
