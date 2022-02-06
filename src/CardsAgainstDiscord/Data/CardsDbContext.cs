using Microsoft.EntityFrameworkCore;

namespace CardsAgainstDiscord.Data;

public class CardsDbContext : DbContext
{
    public CardsDbContext(DbContextOptions<CardsDbContext> options) : base(options)
    {
    }
}