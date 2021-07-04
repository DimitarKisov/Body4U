namespace Body4U.Services.Data.Contracts
{
    using Body4U.Data.ClaimsProvider;
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Web.ViewModels.Account;
    using Microsoft.AspNetCore.Identity;
    using SendGrid;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAccountService
    {
        Task<GlobalResponseData<ApplicationUser>> Register(RegisterRequest model);

        Task<bool> Login(LoginRequest model);

        Task<IdentityResult> ChangePassword(ChangePasswordRequest model, ApplicationUser user);

        Task<GlobalResponseData<EditMyProfileRequest>> MyProfile(ApplicationUser user);

        //GlobalResponseData<EditMyProfileRequest> EditMyProfile(ApplicationUser loggedInUser);

        Task<GlobalResponseData<bool>> MyProfile(EditMyProfileRequest model, ApplicationUser loggedInUser);

        Task<GlobalResponseData<List<MyArticlesViewModel>>> MyArticles(string userId, IGetClaimsProvider claimsProvider);

        Task<Response> SendEmailConfirmation(string email, string confirmationLink);

        Task<Response> SendEmailResetPassword(string email, string passwordResetLink);
    }
}
