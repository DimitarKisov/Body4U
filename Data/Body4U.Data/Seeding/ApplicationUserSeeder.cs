namespace Body4U.Data.Seeding
{
    using Body4U.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ApplicationUserSeeder : ISeeder
    {
        private readonly IConfiguration configuration;

        public ApplicationUserSeeder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userName = configuration.GetSection("SeedInfo")["UserName"];
            var passsword = configuration.GetSection("SeedInfo")["Password"];

            await SeedUserAsync(userManager, userName, passsword);
        }

        private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, string userName, string password)
        {
            var user = await userManager.FindByEmailAsync(userName);

            if (user == null)
            {
                var result = await userManager.CreateAsync(new ApplicationUser()
                {
                    FirstName = "Body4U",
                    LastName = "Admin",
                    Email = userName,
                    UserName = userName,
                    EmailConfirmed = true
                }, password);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
