using CardsAgainstDiscord;
using CardsAgainstDiscord.Data;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        var configuration = host.Configuration;
        
        services.AddDbContextFactory<CardsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();