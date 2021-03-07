using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(SportClubsChallenges.AzureFunctions.IoC.Startup))]
namespace SportClubsChallenges.AzureFunctions.IoC
{
    using AutoMapper;
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
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var context = builder.GetContext();
            var connectionstring = context.Configuration.GetConnectionString("SportClubsChallengesDbConnString");

            builder.Services.AddAutoMapper(typeof(DtoModelMappingsProfile));
            builder.Services.AddAutoMapper(typeof(StravaModelMappingsProfile));

            builder.Services.AddDbContext<SportClubsChallengesDbContext>(options => options.UseSqlServer(connectionstring));

            builder.Services.AddHttpClient<StravaApiWrapper>();

            builder.Services.AddScoped<IStravaApiWrapper, StravaApiWrapper>();
            builder.Services.AddScoped<ITokenService, TokenService>();
        }
    }
}
