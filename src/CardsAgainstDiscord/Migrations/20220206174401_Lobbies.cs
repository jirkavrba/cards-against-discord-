using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CardsAgainstDiscord.Migrations
{
    public partial class Lobbies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    table.PrimaryKey("PK_lobbies", x => x.id);
                    table.UniqueConstraint("AK_lobbies_guild_id_channel_id_message_id", x => new { x.guild_id, x.channel_id, x.message_id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lobbies");
        }
    }
}
