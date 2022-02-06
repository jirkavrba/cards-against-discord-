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

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        model.Entity<Lobby>().HasAlternateKey(l => new {l.GuildId, l.ChannelId, l.MessageId});

        model.Entity<Player>()
            .HasMany(p => p.WhiteCards)
            .WithMany(c => c.Players);
    }
}