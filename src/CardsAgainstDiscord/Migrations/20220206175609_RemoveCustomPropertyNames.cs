using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardsAgainstDiscord.Migrations
{
    public partial class RemoveCustomPropertyNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_lobbies_guild_id_channel_id_message_id",
                table: "lobbies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lobbies",
                table: "lobbies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_white_cards",
                table: "white_cards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_black_cards",
                table: "black_cards");

            migrationBuilder.RenameTable(
                name: "lobbies",
                newName: "Lobbies");

            migrationBuilder.RenameTable(
                name: "white_cards",
                newName: "WhiteCards");

            migrationBuilder.RenameTable(
                name: "black_cards",
                newName: "BlackCards");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Lobbies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Lobbies",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "Lobbies",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "joined_players",
                table: "Lobbies",
                newName: "JoinedPlayers");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Lobbies",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "channel_id",
                table: "Lobbies",
                newName: "ChannelId");

            migrationBuilder.RenameColumn(
                name: "text",
                table: "WhiteCards",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "WhiteCards",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "text",
                table: "BlackCards",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "picks",
                table: "BlackCards",
                newName: "Picks");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "BlackCards",
                newName: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Lobbies_GuildId_ChannelId_MessageId",
                table: "Lobbies",
                columns: new[] { "GuildId", "ChannelId", "MessageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lobbies",
                table: "Lobbies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WhiteCards",
                table: "WhiteCards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlackCards",
                table: "BlackCards",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Lobbies_GuildId_ChannelId_MessageId",
                table: "Lobbies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lobbies",
                table: "Lobbies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WhiteCards",
                table: "WhiteCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlackCards",
                table: "BlackCards");

            migrationBuilder.RenameTable(
                name: "Lobbies",
                newName: "lobbies");

            migrationBuilder.RenameTable(
                name: "WhiteCards",
                newName: "white_cards");

            migrationBuilder.RenameTable(
                name: "BlackCards",
                newName: "black_cards");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "lobbies",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "lobbies",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "lobbies",
                newName: "message_id");

            migrationBuilder.RenameColumn(
                name: "JoinedPlayers",
                table: "lobbies",
                newName: "joined_players");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "lobbies",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "lobbies",
                newName: "channel_id");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "white_cards",
                newName: "text");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "white_cards",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "black_cards",
                newName: "text");

            migrationBuilder.RenameColumn(
                name: "Picks",
                table: "black_cards",
                newName: "picks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "black_cards",
                newName: "id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_lobbies_guild_id_channel_id_message_id",
                table: "lobbies",
                columns: new[] { "guild_id", "channel_id", "message_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_lobbies",
                table: "lobbies",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_white_cards",
                table: "white_cards",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_black_cards",
                table: "black_cards",
                column: "id");
        }
    }
}
