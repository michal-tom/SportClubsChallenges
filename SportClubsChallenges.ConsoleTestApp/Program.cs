namespace SportClubsChallenges.ConsoleTestApp
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Domain.Services;
    using SportClubsChallenges.Jobs;
    using SportClubsChallenges.Strava;
    using SportClubsChallenges.Mappings;
    using System;
    using System.Threading.Tasks;

    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("SportClubsChallenges Console Test App");

            var services = ConfigureServices();

           // await GetActivities(services);
           // await DeactivateChallenges(services);
            await UpdateClassifications(services);

            Console.ReadKey();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddAutoMapper(typeof(DtoModelMappingsProfile));
            services.AddAutoMapper(typeof(StravaModelMappingsProfile));

            // TODO: get connection string from config
            services.AddDbContext<SportClubsChallengesDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SportClubsChallenges;Trusted_Connection=True;MultipleActiveResultSets=true"));

            services.AddHttpClient<StravaApiWrapper>();

            services.AddScoped<IStravaApiWrapper, StravaApiWrapper>();
            services.AddScoped<ITokenService, TokenService>();

            return services.BuildServiceProvider();
        }

        private static async Task GetActivities(ServiceProvider services)
        {
            var dbContext = services.GetRequiredService<SportClubsChallengesDbContext>();
            var stravaWrapper = services.GetRequiredService<IStravaApiWrapper>();
            var tokenService = services.GetRequiredService<ITokenService>();
            var mapper = services.GetRequiredService<IMapper>();

            var job = new GetAthleteActivitiesJob(dbContext, stravaWrapper, tokenService, mapper);
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
