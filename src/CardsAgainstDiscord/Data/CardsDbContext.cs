using CardsAgainstDiscord.Model;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstDiscord.Data;

public class CardsDbContext : DbContext
{
    public CardsDbContext(DbContextOptions<CardsDbContext> options) : base(options)
    {
    }

    public virtual DbSet<BlackCard> BlackCards { get; protected set; } = null!;

    public virtual DbSet<WhiteCard> WhiteCards { get; protected set; } = null!;

    public virtual DbSet<Lobby> Lobbies { get; protected set; } = null!;

    public virtual DbSet<Player> Players { get; protected set; } = null!;

    public virtual DbSet<Game> Games { get; protected set; } = null!;

    public virtual DbSet<GameRound> Rounds { get; protected set; } = null!;

    public virtual DbSet<PickedCard> Picks { get; protected set; } = null!;

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        model.Entity<Lobby>().HasAlternateKey(l => new {l.GuildId, l.ChannelId, l.MessageId});

        model.Entity<Player>()
            .HasMany(p => p.WhiteCards)
            .WithMany(c => c.Players);

        model.Entity<Player>()
            .HasMany(p => p.JudgingRounds)
            .WithOne(r => r.Judge)
            .IsRequired()
            .HasForeignKey(r => r.JudgeId);

        model.Entity<Player>()
            .HasMany(p => p.PickedCards)
            .WithOne(c => c.Player)
            .HasForeignKey(c => c.PlayerId);

        model.Entity<Game>()
            .HasOne(g => g.CurrentRound)
            .WithOne(r => r.Game)
            .HasForeignKey<GameRound>(r => r.GameId);

        model.Entity<Game>()
            .HasMany(g => g.Players)
            .WithOne(p => p.Game)
            .HasForeignKey(p => p.GameId);

        model.Entity<GameRound>()
            .HasOne(r => r.BlackCard)
            .WithMany(c => c.Rounds)
            .HasForeignKey(r => r.BlackCardId);

        model.Entity<GameRound>()
            .HasMany(r => r.PickedCards)
            .WithOne(p => p.Round)
            .HasForeignKey(p => p.RoundId);

        model.Entity<PickedCard>()
            .HasOne(c => c.WhiteCard)
            .WithMany(c => c.Picks)
            .HasForeignKey(c => c.WhiteCardId);

        model.Entity<PickedCard>()
            .HasOne(c => c.Player)
            .WithMany(p => p.PickedCards)
            .HasForeignKey(c => c.PlayerId);
    }
}