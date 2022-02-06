using CardsAgainstDiscord.Configuration;
using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Extensions;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((host, builder) =>
    {
        builder.AddEnvironmentVariables();

        if (host.HostingEnvironment.IsDevelopment())
        {
            builder.AddUserSecrets<Program>();
        }
    })
    .ConfigureServices((host, services) =>
    {
        var configuration = host.Configuration;
        
        services.Configure<DiscordConfiguration>(configuration.GetRequiredSection(DiscordConfiguration.Section));

        services.AddDbContextFactory<CardsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddDiscordBot();
        services.AddSlashCommands();
    })
    .Build();

await host.RunAsync();