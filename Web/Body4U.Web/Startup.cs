namespace Body4U.Web
{
    using System;
    using System.Linq;
    using Body4U.Data;
    using Body4U.Data.Models;
    using Body4U.Data.Seeding;
    using Body4U.Services.Data.Contracts;
    using Body4U.Services.Data.Services;
    using Body4U.Services.Messaging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            SeedIdentityData(services, this.configuration);

            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services.ConfigureApplicationCookie(
                options =>
                {
                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.LoginPath = "/Account/Login";
                    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                    //options.AccessDeniedPath = "/Home/HttpError";
                    options.SlidingExpiration = true;
                });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .AddRazorRuntimeCompilation()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            services.AddAntiforgery(
                options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });

            services.AddRazorPages();
            services.AddCloudscribePagination();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSingleton(configuration);

            // Application services
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IArticleService, ArticleService>();
            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(configuration.GetSection("SendGrid")["ApiKey"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home");
                app.UseStatusCodePagesWithReExecute("/Home/{0}");
                app.UseHsts();
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("bg-BG")
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });
        }

        public static void SeedIdentityData(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();

            using (var dbContext = (ApplicationDbContext)serviceProvider.GetService(typeof(ApplicationDbContext)))
            {
                if (!(dbContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                {
                    dbContext.Database.Migrate();
                }

                if (!dbContext.Users.IgnoreQueryFilters().Any())
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        new ApplicationDbContextSeeder(configuration).SeedAsync(dbContext, serviceProvider).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}
