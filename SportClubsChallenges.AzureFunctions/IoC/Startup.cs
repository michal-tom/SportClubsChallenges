using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(SportClubsChallenges.AzureFunctions.IoC.Startup))]
namespace SportClubsChallenges.AzureFunctions.IoC
{
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Domain.Services;
    using SportClubsChallenges.Mappings;
    using SportClubsChallenges.Strava;
    using System.IO;

    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();
            var environmentName = context.EnvironmentName;

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "commonsettings.json"), optional: false, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"commonsettings.{environmentName}.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "connectionstrings.json"), optional: false, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var context = builder.GetContext();
            var connectionstring = context.Configuration.GetConnectionString("SportClubsChallengesDbConnString");

            builder.Services.AddAutoMapper(typeof(DtoModelMappingsProfile));
            builder.Services.AddAutoMapper(typeof(StravaModelMappingsProfile));

            builder.Services.AddDbContext<SportClubsChallengesDbContext>(options => options.UseSqlServer(connectionstring));

            builder.Services.AddScoped<IActivityService, ActivityService>();

            builder.Services.AddHttpClient<StravaApiWrapper>();
            builder.Services.AddHttpClient<StravaSubscriptionService>();

            builder.Services.AddScoped<IStravaApiWrapper, StravaApiWrapper>();
            builder.Services.AddScoped<IStravaSubscriptionService, StravaSubscriptionService> ();
            builder.Services.AddScoped<ITokenService, TokenService>();
        }
    }
}
