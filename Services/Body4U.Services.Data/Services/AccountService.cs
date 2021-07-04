namespace Body4U.Services.Data.Services
{
    using Body4U.Common;
    using Body4U.Data;
    using Body4U.Data.ClaimsProvider;
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Services.Data.Contracts;
    using Body4U.Web.ViewModels.Account;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
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

        public async Task<GlobalResponseData<ApplicationUser>> Register(RegisterRequest model)
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
                    return GlobalResponseData<ApplicationUser>.BadResponse(GlobalConstants.WrongImageFormat);
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
                    return GlobalResponseData<ApplicationUser>.CorrectResponse(user);
                }

                return GlobalResponseData<ApplicationUser>.BadResponse(GlobalConstants.RegistrationUnssuccesful);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AccountService: Register");
                return GlobalResponseData<ApplicationUser>.BadResponse(GlobalConstants.Wrong);
            }
            
        }

        public async Task<bool> Login(LoginRequest model)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AccountService: Login");
                return false;
            }
        }

        public async Task<IdentityResult> ChangePassword(ChangePasswordRequest model, ApplicationUser user)
        {
            try
            {
                var result = await userManager.ChangePasswordAsync(user,
                model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    return IdentityResult.Success;
                }

                var errors = result.Errors as IdentityError[];
                return IdentityResult.Failed(errors);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AccountService: ChangePassword");
                return IdentityResult.Failed();
            }
        }

        public async Task<GlobalResponseData<EditMyProfileRequest>> MyProfile(ApplicationUser user)
        {
            try
            {
                var result = new EditMyProfileRequest()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Age = user.Age,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Sex,
                    CurrentProfilePicture = user.ProfilePicture != null ? Convert.ToBase64String(user.ProfilePicture) : null,
                };

                var trainer = await dbContext.Trainers.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);
                if (trainer != null)
                {
                    result.Bio = trainer.Bio;
                    result.ShortBio = trainer.ShortBio;
                    result.FacebookUrl = trainer.FacebookUrl;
                    result.InstagramUrl = trainer.InstagramUrl;
                    result.YoutubeChannelUrl = trainer.YoutubeChannelUrl;
                }

                return GlobalResponseData<EditMyProfileRequest>.CorrectResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AccountService: MyProfile");
                return GlobalResponseData<EditMyProfileRequest>.BadResponse(GlobalConstants.Wrong);
            }
        }

        //public GlobalResponseData<EditMyProfileRequest> EditMyProfile(ApplicationUser currentlyLoggedInUser)
        //{
        //    try
        //    {
        //        var result = new EditMyProfileRequest()
        //        {
        //            Id = currentlyLoggedInUser.Id,
        //            FirstName = currentlyLoggedInUser.FirstName,
        //            LastName = currentlyLoggedInUser.LastName,
        //            Age = currentlyLoggedInUser.Age,
        //            PhoneNumber = currentlyLoggedInUser.PhoneNumber
        //        };

        //        var trainer = dbContext.Trainers.FirstOrDefault(x => x.ApplicationUserId == currentlyLoggedInUser.Id);
        //        if (trainer != null)
        //        {
        //            result.Bio = trainer.Bio;
        //            result.ShortBio = trainer.ShortBio;
        //            result.FacebookUrl = trainer.FacebookUrl;
        //            result.InstagramUrl = trainer.InstagramUrl;
        //            result.YoutubeChannelUrl = trainer.YoutubeChannelUrl;
        //            result.IsReadyToWrite = trainer.IsReadyToWrite;
        //        }

        //        return GlobalResponseData<EditMyProfileRequest>.CorrectResponse(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "AccountService: GetMyProfileForEdit");
        //        return GlobalResponseData<EditMyProfileRequest>.BadResponse(GlobalConstants.Wrong);
        //    }
        //}

        public async Task<GlobalResponseData<bool>> MyProfile(EditMyProfileRequest model, ApplicationUser currentlyLoggedInUser)
        {
            try
            {
                if (model.ProfilePicture != null && model.ProfilePicture.ContentType != "image/jpeg" && model.ProfilePicture.ContentType != "image/png" && model.ProfilePicture.ContentType != "image/jpg")
                {
                    return GlobalResponseData<bool>.BadResponse(GlobalConstants.WrongImageFormat);
                }

                currentlyLoggedInUser.FirstName = model.FirstName;
                currentlyLoggedInUser.LastName = model.LastName;
                currentlyLoggedInUser.Age = model.Age;
                currentlyLoggedInUser.PhoneNumber = model.PhoneNumber;

                if (model.ProfilePicture != null)
                {
                    if (model.ProfilePicture.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await model.ProfilePicture.CopyToAsync(stream);

                            if (currentlyLoggedInUser.ProfilePicture != stream.ToArray())
                            {
                                currentlyLoggedInUser.ProfilePicture = stream.ToArray();
                            }
                        }
                    }
                }

                var trainer = dbContext.Trainers.FirstOrDefault(x => x.ApplicationUserId == currentlyLoggedInUser.Id);
                if (trainer != null && await userManager.IsInRoleAsync(currentlyLoggedInUser, GlobalConstants.TrainerRoleName))
                {
                    trainer.Bio = model.Bio;
                    trainer.ShortBio = model.ShortBio;
                    trainer.FacebookUrl = model.FacebookUrl;
                    trainer.InstagramUrl = model.InstagramUrl;
                    trainer.YoutubeChannelUrl = model.YoutubeChannelUrl;

                    if (trainer.ShortBio != null && trainer.Bio != null /*&& trainerVideos > 0 && trainerImages > 0*/)
                    {
                        trainer.IsReadyToVisualize = true;
                        trainer.IsReadyToWrite = true;
                    }
                }

                await dbContext.SaveChangesAsync();

                return GlobalResponseData<bool>.CorrectResponse(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AccountService: EditMyProfilForEdit");
                return GlobalResponseData<bool>.BadResponse(GlobalConstants.Wrong);
            }
        }

        public async Task<GlobalResponseData<List<MyArticlesViewModel>>> MyArticles(string userId, IGetClaimsProvider claimsProvider)
        {
            try
            {
                if (userId != claimsProvider.UserId && claimsProvider.IsTrainer.HasValue && !claimsProvider.IsAdmin.HasValue)
                {
                    return GlobalResponseData<List<MyArticlesViewModel>>.BadResponse(GlobalConstants.WrongRights);
                }
                else if (userId != claimsProvider.UserId && !claimsProvider.IsTrainer.HasValue && !claimsProvider.IsAdmin.HasValue)
                {
                    return GlobalResponseData<List<MyArticlesViewModel>>.BadResponse(GlobalConstants.NotFound);
                }

                var articles = await dbContext.Articles
                    .Where(a => a.ApplicationUserId == userId)
                    .OrderByDescending(a => a.DatePosted)
                    .Select(a => new MyArticlesViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Content = a.Content,
                        Image = Convert.ToBase64String(a.Image),
                        DatePosted = a.DatePosted.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    })
                    .ToListAsync();


                return GlobalResponseData<List<MyArticlesViewModel>>.CorrectResponse(articles);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AccountService: MyArticles");
                return GlobalResponseData<List<MyArticlesViewModel>>.BadResponse(GlobalConstants.Wrong);
            }
        }

        public async Task<Response> SendEmailConfirmation(string email, string confirmationLink)
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

        public async Task<Response> SendEmailResetPassword(string email, string passwordResetLink)
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
