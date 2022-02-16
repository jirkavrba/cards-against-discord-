using CardsAgainstDiscord.Configuration;
using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Extensions;
using CardsAgainstDiscord.Services;
using CardsAgainstDiscord.Services.Contracts;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((host, builder) =>
    {
        builder.AddEnvironmentVariables();

        if (host.HostingEnvironment.IsDevelopment())
        {
            builder.AddUserSecrets<Program>(optional: true);
        }
    })
    .ConfigureServices((host, services) =>
    {
        var configuration = host.Configuration;

        services.Configure<DiscordConfiguration>(configuration.GetRequiredSection(DiscordConfiguration.Section));

        services.AddDbContext<CardsDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("Default"))
                .UseSnakeCaseNamingConvention(),
            ServiceLifetime.Transient,
            ServiceLifetime.Singleton
        );
        services.AddDbContextFactory<CardsDbContext>();

        services.AddTransient<ILobbiesService, LobbiesService>();
        services.AddTransient<IGamesService, GamesService>();

        services.AddDiscordBot();
        services.AddSlashCommands();

        services.AddHostedService<CardsImportingService>();
    })
    .Build();

await host.RunAsync();