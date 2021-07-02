namespace Body4U.Web.Infrastructure
{
    using Body4U.Data;
    using Body4U.Data.ClaimsProvider;
    using Body4U.Data.Models;
    using Body4U.Data.Seeding;
    using Body4U.Services.Data.Contracts;
    using Body4U.Services.Data.Services;
    using Body4U.Services.Messaging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using System;
    using System.Linq;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection SeedIdentityData(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();

            using (var dbContext = (ApplicationDbContext)serviceProvider.GetService(typeof(ApplicationDbContext)))
            {
                if (!(dbContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                {
                    dbContext.Database.Migrate();
                }

                if (!dbContext.Users.Any())
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            new ApplicationDbContextSeeder(configuration).SeedAsync(dbContext, serviceProvider).GetAwaiter().GetResult();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "ServiceCollectionExtensions: SeedIdentityData");
                        }
                    }
                }
            }

            return services;
        }

        public static IServiceCollection ConfigureCookiePolicyOption(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            return services;
        }

        public static IServiceCollection ConfigureApplicationCookie(this IServiceCollection services)
        {
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

            return services;
        }

        public static IServiceCollection AddLocalizations(this IServiceCollection services)
        {
            return services.AddLocalization(options => options.ResourcesPath = "Resources");
        }

        public static IServiceCollection AddControllersWtihViews(this IServiceCollection services)
        {
            services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .AddRazorRuntimeCompilation()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            return services;
        }

        public static IServiceCollection AddAntiForgery(this IServiceCollection services)
        {
            services.AddAntiforgery(
                options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddScoped<IAccountService, AccountService>()
               .AddScoped<IArticleService, ArticleService>()
               .AddScoped<IGetClaimsProvider, GetClaimsProvider>()
               .AddScoped<IEmailSender>(x => new SendGridEmailSender(configuration.GetSection("SendGrid")["ApiKey"]));

            return services;
        }
    }
}
