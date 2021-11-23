namespace SportClubsChallenges.AzureFunctions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Domain.Services;
    using SportClubsChallenges.Mappings;
    using SportClubsChallenges.Strava;
    using System;

    public  class Program
    {
        public static void Main()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var host = new HostBuilder()
                .ConfigureAppConfiguration(configurationBuilder => configurationBuilder
                    .AddJsonFile("commonsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"commonsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("connectionstrings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                )
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((context, services) =>
                {
                    var connectionstring = context.Configuration.GetConnectionString("SportClubsChallengesDbConnString");

                    services.AddAutoMapper(typeof(DtoModelMappingsProfile));
                    services.AddAutoMapper(typeof(StravaModelMappingsProfile));

                    services.AddDbContext<SportClubsChallengesDbContext>(options => options.UseSqlServer(connectionstring));

                    services.AddScoped<IActivityService, ActivityService>();
                    services.AddScoped<INotificationService, NotificationService>();

                    services.AddHttpClient<StravaApiWrapper>();
                    services.AddHttpClient<StravaSubscriptionService>();

                    services.AddScoped<IStravaApiWrapper, StravaApiWrapper>();
                    services.AddScoped<IStravaSubscriptionService, StravaSubscriptionService>();
                    services.AddScoped<ITokenService, TokenService>();
                })
                .Build();

            host.Run();
        }
    }
}
