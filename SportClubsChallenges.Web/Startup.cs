namespace SportClubsChallenges.Web
{
    using AutoMapper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Services;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Domain.Mappings;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using AspNet.Security.OAuth.Strava;
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

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
                options.ClientId = "60033";
                options.ClientSecret = "45b4066142165ecd3dee2d28556da83d77081bea";
                options.Scope.Add("activity:read");
                options.Scope.Add("activity:read_all");
                options.Scope.Add("profile:read_all");
                options.SaveTokens = true;

                options.Events.OnTicketReceived = async ctx =>
                {
                    var user = (ClaimsIdentity) ctx.Principal.Identity;
                    if (user.IsAuthenticated)
                    {
                        var athleteService = ctx.HttpContext.RequestServices.GetService<IAthleteService>();
                        await athleteService.OnAthleteLogin(user, ctx.Properties.GetTokens());
                    }

                    return;

                    //List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                    //tokens.Add(new AuthenticationToken()
                    //{
                    //    Name = "TicketCreated",
                    //    Value = DateTime.UtcNow.ToString()
                    //});

                    //ctx.Properties.StoreTokens(tokens);

                    //return Task.CompletedTask;
                };
            });

            services.AddHttpContextAccessor();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddAutoMapper(typeof(ModelMappingsProfile));

            services.AddDbContext<SportClubsChallengesDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddHttpClient<StravaTokenService>();

            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IChallengeService, ChallengeService>();
            services.AddScoped<IAthleteService, AthleteService>();
            services.AddScoped<IStravaTokenService, StravaTokenService>();
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
    }
}
