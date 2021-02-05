namespace Body4U.Services.Data.Services
{
    using Body4U.Common;
    using Body4U.Data;
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Services.Data.Contracts;
    using Body4U.Web.ViewModels.Account;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public AccountService(ApplicationDbContext dbContext,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        public async Task<ResponseData<ApplicationUser>> Register(RegisterRequest model)
        {
            try
            {
                var user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Age = model.Age,
                    PhoneNumber = model.PhoneNumber,
                    Sex = model.Gender
                };

                if (model.ProfilePicture != null && model.ProfilePicture.ContentType != "image/jpeg" && model.ProfilePicture.ContentType != "image/png")
                {
                    return ResponseData<ApplicationUser>.BadResponse(GlobalConstants.WrongImageFormat);
                }

                if (model.ProfilePicture != null)
                {
                    if (model.ProfilePicture.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            model.ProfilePicture.CopyTo(stream);
                            user.ProfilePicture = stream.ToArray();
                        }
                    }
                }

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);
                    return ResponseData<ApplicationUser>.CorrectResponse(user);
                }

                return ResponseData<ApplicationUser>.BadResponse(GlobalConstants.Wrong);
            }
            catch (Exception)
            {
                return ResponseData<ApplicationUser>.BadResponse(GlobalConstants.Wrong);
            }
            
        }

        public async Task<bool> Login(LoginRequest model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest model, ApplicationUser user)
        {
            var result = await userManager.ChangePasswordAsync(user,
                model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);
                return true;
            }

            return false;
        }

        public async Task<SendGrid.Response> SendEmailConfirmation(string email, string confirmationLink)
        {
            var apiKey = configuration.GetSection("SendGrid")["ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(configuration.GetSection("SendGrid")["Sender"], "Body4U Admin");
            var subject = "Email Confirmation";
            var to = new EmailAddress(email);
            var htmlContent = $"<p>За да потвърдите, моля кликлнете <a href=\"{confirmationLink}\">ТУК</a></p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            return await client.SendEmailAsync(msg);
        }

        public async Task<SendGrid.Response> SendEmailResetPassword(string email, string passwordResetLink)
        {
            var apiKey = configuration.GetSection("SendGrid")["ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(configuration.GetSection("SendGrid")["Sender"], "Body4U Admin");
            var subject = "Password Reset";
            var to = new EmailAddress(email);
            var htmlContent = $"<p>За да подновите, моля кликлнете <a href=\"{passwordResetLink}\">ТУК</a></p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            return await client.SendEmailAsync(msg);
        }
    }
}
