namespace SportClubsChallenges.ConsoleTestApp
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using AutoMapper;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Domain.Services;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Mappings;
    using SportClubsChallenges.Strava;

    public class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("SportClubsChallenges Console Test App");

            var services = ConfigureServices();

            Console.WriteLine("Configuring - DONE");

            await GetClubs(services);

            Console.WriteLine("Get clubs - DONE");

            await GetActivities(services);

            Console.WriteLine("Get activities - DONE");

            await DeactivateChallenges(services);

            Console.WriteLine("Deactivate challenges - DONE");

            await UpdateClassifications(services);

            Console.WriteLine("Update classifications - DONE");
        }

        private static ServiceProvider ConfigureServices()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
               .AddJsonFile("commonsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile("connectionstrings.json", optional: false, reloadOnChange: true)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();

            var connectionstring = configuration.GetConnectionString("SportClubsChallengesDbConnString");

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(provider => configuration);

            services.AddAutoMapper(typeof(DtoModelMappingsProfile));
            services.AddAutoMapper(typeof(StravaModelMappingsProfile));

            services.AddDbContext<SportClubsChallengesDbContext>(options => options.UseSqlServer(connectionstring));

            services.AddScoped<IActivityService, ActivityService>();

            services.AddHttpClient<StravaApiWrapper>();

            services.AddScoped<IStravaApiWrapper, StravaApiWrapper>();
            services.AddScoped<ITokenService, TokenService>();

            return services.BuildServiceProvider();
        }

        private static async Task GetClubs(ServiceProvider services)
        {
            var dbContext = services.GetRequiredService<SportClubsChallengesDbContext>();
            var stravaWrapper = services.GetRequiredService<IStravaApiWrapper>();
            var tokenService = services.GetRequiredService<ITokenService>();
            var mapper = services.GetRequiredService<IMapper>();

            var job = new GetAthletesClubsJob(dbContext, stravaWrapper, tokenService, mapper);
            await job.Run();
        }

        private static async Task GetActivities(ServiceProvider services)
        {
            var dbContext = services.GetRequiredService<SportClubsChallengesDbContext>();
            var stravaWrapper = services.GetRequiredService<IStravaApiWrapper>();
            var activityService = services.GetRequiredService<IActivityService>();
            var tokenService = services.GetRequiredService<ITokenService>();
            var mapper = services.GetRequiredService<IMapper>();

            var job = new GetAthletesActivitiesJob(dbContext, activityService, stravaWrapper, tokenService, mapper);
            await job.Run();
        }

        private static async Task DeactivateChallenges(ServiceProvider services)
        {
            var dbContext = services.GetRequiredService<SportClubsChallengesDbContext>();

            var job = new DeactivateChallengesJob(dbContext);
            await job.Run();
        }

        private static async Task UpdateClassifications(ServiceProvider services)
        {
            var dbContext = services.GetRequiredService<SportClubsChallengesDbContext>();

            var job = new UpdateChallengesClassificationsJob(dbContext);
            await job.Run();
        }
    }
}