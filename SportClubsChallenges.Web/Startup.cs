namespace SportClubsChallenges.Web
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AspNet.Security.OAuth.Strava;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Blazored.LocalStorage;
    using SportClubsChallenges.AzureQueues;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Domain.Services;
    using SportClubsChallenges.Mappings;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = StravaAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddStrava(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClientId = Configuration["StravaClientId"];
                options.ClientSecret = Configuration["StravaClientSecret"];
                options.Scope.Add("activity:read");
                options.Scope.Add("activity:read_all");
                options.Scope.Add("profile:read_all");
                options.SaveTokens = true;
                options.Events.OnTicketReceived += SignUpUser;
            });

            services.AddHttpContextAccessor();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddBlazoredLocalStorage();

            services.AddAutoMapper(typeof(DtoModelMappingsProfile));

            services.AddDbContext<SportClubsChallengesDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SportClubsChallengesDbConnString")), ServiceLifetime.Transient);

            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IChallengeService, ChallengeService>();
            services.AddScoped<IAthleteService, AthleteService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAzureStorageRepository, AzureStorageRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private async Task SignUpUser(TicketReceivedContext ticketReceivedContext)
        {
            var user = (ClaimsIdentity) ticketReceivedContext.Principal.Identity;
            if (user.IsAuthenticated)
            {
                var athleteService = ticketReceivedContext.HttpContext.RequestServices.GetService<IAthleteService>();
                await athleteService.OnAthleteLogin(user, ticketReceivedContext.Properties);
            }

            return;
        }
    }
}