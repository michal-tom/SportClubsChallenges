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

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddAutoMapper(typeof(ModelMappingsProfile));

            services.AddDbContext<SportClubsChallengesDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IClubService, ClubService>();

            services.AddScoped<IChallengeService, ChallengeService>();

            //services.AddDbContext<DataAccess.AppContext>(options =>
            //              options.UseSqlServer(
            //                  Configuration.GetConnectionString("DefaultConnection2")));

            ////Article service  
            //services.AddScoped<IArticleManager, ArticleManager>();
            ////Register dapper in scope  
            //services.AddScoped<IDapperManager, DapperManager>();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
