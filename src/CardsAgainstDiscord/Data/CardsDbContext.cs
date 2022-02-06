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
}