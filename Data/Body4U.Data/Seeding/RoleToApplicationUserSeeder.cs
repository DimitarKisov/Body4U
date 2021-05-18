namespace Body4U.Data.Seeding
{
    using Body4U.Common;
    using Body4U.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class RoleToApplicationUserSeeder : ISeeder
    {
        private readonly IConfiguration configuration;

        public RoleToApplicationUserSeeder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userName = configuration.GetSection("SeedInfo")["UserName"];

            await AssignRoles(userManager, dbContext, userName, GlobalConstants.AdministratorRoleName);
            await AssignRoles(userManager, dbContext, userName, GlobalConstants.TrainerRoleName);
        }

        public static async Task AssignRoles(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, string email, string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var result = await userManager.AddToRoleAsync(user, role);

                if (result.Succeeded)
                {
                    if (role == GlobalConstants.TrainerRoleName)
                    {
                        
                        if (!dbContext.Trainers.Any(x => x.ApplicationUserId == user.Id))
                        {
                            var trainer = new Trainer
                            {
                                Bio = null,
                                ShortBio = null,
                                TrainerImages = null,
                                TrainerVideos = null,
                                FacebookUrl = null,
                                InstagramUrl = null,
                                YoutubeChannelUrl = null,
                                IsReadyToVisualize = false,
                                //CreatedOn = DateTime.Now,
                                ApplicationUserId = user.Id
                            };

                            dbContext.Trainers.Add(trainer);
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
