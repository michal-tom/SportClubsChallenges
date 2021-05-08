namespace SportClubsChallenges.Api
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
                        .AddJsonFile(Path.Combine(solutionMainFolder, "connectionstrings.json"), optional: true, reloadOnChange: true)
                        // When app is published
                        .AddJsonFile("connectionstrings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    configBuilder.AddEnvironmentVariables();
                });
    }
}