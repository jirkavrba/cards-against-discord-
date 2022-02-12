﻿// <auto-generated />
using System;
using CardsAgainstDiscord.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CardsAgainstDiscord.Migrations
{
    [DbContext(typeof(CardsDbContext))]
    partial class CardsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BlackCardGame", b =>
                {
                    b.Property<int>("GamesId")
                        .HasColumnType("integer")
                        .HasColumnName("games_id");

                    b.Property<int>("UsedBlackCardsId")
                        .HasColumnType("integer")
                        .HasColumnName("used_black_cards_id");

                    b.HasKey("GamesId", "UsedBlackCardsId")
                        .HasName("pk_black_card_game");

                    b.HasIndex("UsedBlackCardsId")
                        .HasDatabaseName("ix_black_card_game_used_black_cards_id");

                    b.ToTable("black_card_game", (string)null);
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.BlackCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Picks")
                        .HasColumnType("integer")
                        .HasColumnName("picks");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("text");

                    b.HasKey("Id")
                        .HasName("pk_black_cards");

                    b.ToTable("black_cards", (string)null);
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BlackCardId")
                        .HasColumnType("integer")
                        .HasColumnName("black_card_id");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal[]>("JoiningPlayers")
                        .IsRequired()
                        .HasColumnType("numeric(20,0)[]")
                        .HasColumnName("joining_players");

                    b.Property<int?>("JudgeId")
                        .HasColumnType("integer")
                        .HasColumnName("judge_id");

                    b.Property<decimal[]>("LeavingPlayers")
                        .IsRequired()
                        .HasColumnType("numeric(20,0)[]")
                        .HasColumnName("leaving_players");

                    b.Property<decimal?>("MessageId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("message_id");

                    b.Property<int?>("SelectedWinnerId")
                        .HasColumnType("integer")
                        .HasColumnName("selected_winner_id");

                    b.Property<int>("WinPoints")
                        .HasColumnType("integer")
                        .HasColumnName("win_points");

                    b.HasKey("Id")
                        .HasName("pk_games");

                    b.HasIndex("BlackCardId")
                        .HasDatabaseName("ix_games_black_card_id");

                    b.HasIndex("JudgeId")
                        .HasDatabaseName("ix_games_judge_id");

                    b.ToTable("games", (string)null);
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.Lobby", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal[]>("JoinedPlayers")
                        .IsRequired()
                        .HasColumnType("numeric(20,0)[]")
                        .HasColumnName("joined_players");

                    b.Property<decimal>("MessageId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("message_id");

                    b.Property<decimal>("OwnerId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("owner_id");

                    b.Property<int>("WinPoints")
                        .HasColumnType("integer")
                        .HasColumnName("win_points");

                    b.HasKey("Id")
                        .HasName("pk_lobbies");

                    b.HasAlternateKey("GuildId", "ChannelId", "MessageId")
                        .HasName("ak_lobbies_guild_id_channel_id_message_id");

                    b.ToTable("lobbies", (string)null);
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.PickedCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer")
                        .HasColumnName("player_id");

                    b.Property<int>("WhiteCardId")
                        .HasColumnType("integer")
                        .HasColumnName("white_card_id");

                    b.HasKey("Id")
                        .HasName("pk_picks");

                    b.HasIndex("PlayerId")
                        .HasDatabaseName("ix_picks_player_id");

                    b.HasIndex("WhiteCardId")
                        .HasDatabaseName("ix_picks_white_card_id");

                    b.ToTable("picks", (string)null);
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("GameId")
                        .HasColumnType("integer")
                        .HasColumnName("game_id");

                    b.Property<int>("Score")
                        .HasColumnType("integer")
                        .HasColumnName("score");

                    b.Property<int?>("SelectedWhiteCardId")
                        .HasColumnType("integer")
                        .HasColumnName("selected_white_card_id");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_players");

                    b.HasIndex("GameId")
                        .HasDatabaseName("ix_players_game_id");

                    b.HasIndex("SelectedWhiteCardId")
                        .HasDatabaseName("ix_players_selected_white_card_id");

                    b.ToTable("players", (string)null);
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.WhiteCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("text");

                    b.HasKey("Id")
                        .HasName("pk_white_cards");

                    b.ToTable("white_cards", (string)null);
                });

            modelBuilder.Entity("GameWhiteCard", b =>
                {
                    b.Property<int>("GamesId")
                        .HasColumnType("integer")
                        .HasColumnName("games_id");

                    b.Property<int>("UsedWhiteCardsId")
                        .HasColumnType("integer")
                        .HasColumnName("used_white_cards_id");

                    b.HasKey("GamesId", "UsedWhiteCardsId")
                        .HasName("pk_game_white_card");

                    b.HasIndex("UsedWhiteCardsId")
                        .HasDatabaseName("ix_game_white_card_used_white_cards_id");

                    b.ToTable("game_white_card", (string)null);
                });

            modelBuilder.Entity("PlayerWhiteCard", b =>
                {
                    b.Property<int>("PlayersId")
                        .HasColumnType("integer")
                        .HasColumnName("players_id");

                    b.Property<int>("WhiteCardsId")
                        .HasColumnType("integer")
                        .HasColumnName("white_cards_id");

                    b.HasKey("PlayersId", "WhiteCardsId")
                        .HasName("pk_player_white_card");

                    b.HasIndex("WhiteCardsId")
                        .HasDatabaseName("ix_player_white_card_white_cards_id");

                    b.ToTable("player_white_card", (string)null);
                });

            modelBuilder.Entity("BlackCardGame", b =>
                {
                    b.HasOne("CardsAgainstDiscord.Model.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_black_card_game_games_games_id");

                    b.HasOne("CardsAgainstDiscord.Model.BlackCard", null)
                        .WithMany()
                        .HasForeignKey("UsedBlackCardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_black_card_game_black_cards_used_black_cards_id");
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.Game", b =>
                {
                    b.HasOne("CardsAgainstDiscord.Model.BlackCard", "BlackCard")
                        .WithMany()
                        .HasForeignKey("BlackCardId")
                        .HasConstraintName("fk_games_black_cards_black_card_id");

                    b.HasOne("CardsAgainstDiscord.Model.Player", "Judge")
                        .WithMany("JudgedGames")
                        .HasForeignKey("JudgeId")
                        .HasConstraintName("fk_games_players_judge_id");

                    b.Navigation("BlackCard");

                    b.Navigation("Judge");
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.PickedCard", b =>
                {
                    b.HasOne("CardsAgainstDiscord.Model.Player", "Player")
                        .WithMany("PickedCards")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_picks_players_player_id");

                    b.HasOne("CardsAgainstDiscord.Model.WhiteCard", "WhiteCard")
                        .WithMany("Picks")
                        .HasForeignKey("WhiteCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_picks_white_cards_white_card_id");

                    b.Navigation("Player");

                    b.Navigation("WhiteCard");
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.Player", b =>
                {
                    b.HasOne("CardsAgainstDiscord.Model.Game", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_players_games_game_id");

                    b.HasOne("CardsAgainstDiscord.Model.WhiteCard", "SelectedWhiteCard")
                        .WithMany()
                        .HasForeignKey("SelectedWhiteCardId")
                        .HasConstraintName("fk_players_white_cards_selected_white_card_id");

                    b.Navigation("Game");

                    b.Navigation("SelectedWhiteCard");
                });

            modelBuilder.Entity("GameWhiteCard", b =>
                {
                    b.HasOne("CardsAgainstDiscord.Model.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_game_white_card_games_games_id");

                    b.HasOne("CardsAgainstDiscord.Model.WhiteCard", null)
                        .WithMany()
                        .HasForeignKey("UsedWhiteCardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_game_white_card_white_cards_used_white_cards_id");
                });

            modelBuilder.Entity("PlayerWhiteCard", b =>
                {
                    b.HasOne("CardsAgainstDiscord.Model.Player", null)
                        .WithMany()
                        .HasForeignKey("PlayersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_player_white_card_players_players_id");

                    b.HasOne("CardsAgainstDiscord.Model.WhiteCard", null)
                        .WithMany()
                        .HasForeignKey("WhiteCardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_player_white_card_white_cards_white_cards_id");
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.Game", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.Player", b =>
                {
                    b.Navigation("JudgedGames");

                    b.Navigation("PickedCards");
                });

            modelBuilder.Entity("CardsAgainstDiscord.Model.WhiteCard", b =>
                {
                    b.Navigation("Picks");
                });
#pragma warning restore 612, 618
        }
    }
}
