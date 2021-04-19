namespace SportClubsChallenges.Web
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SportClubsChallenges.Database.Data;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<SportClubsChallengesDbContext>();
                context.Database.EnsureCreated();
                //context.Database.Migrate();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    var solutionMainFolder = Path.Combine(env.ContentRootPath, "..");

                    configBuilder.SetBasePath(Directory.GetCurrentDirectory())
                        // When running using dotnet run
                        .AddJsonFile(Path.Combine(solutionMainFolder, "commonsettings.json"), optional: true, reloadOnChange: true)
                        .AddJsonFile(Path.Combine(solutionMainFolder, "connectionstrings.json"), optional: true, reloadOnChange: true)
                        // When app is published
                        .AddJsonFile("commonsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("connectionstrings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    configBuilder.AddEnvironmentVariables();
                });
    }
}
