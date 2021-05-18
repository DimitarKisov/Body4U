namespace Body4U.Services.Data.Contracts
{
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Web.ViewModels.Account;
    using Microsoft.AspNetCore.Identity;
    using SendGrid;
    using System.Threading.Tasks;

    public interface IAccountService
    {
        Task<GlobalResponseData<ApplicationUser>> Register(RegisterRequest model);

        Task<bool> Login(LoginRequest model);

        Task<IdentityResult> ChangePassword(ChangePasswordRequest model, ApplicationUser user);

        GlobalResponseData<MyProfileViewModel> MyProfile(ApplicationUser user);

        GlobalResponseData<EditMyProfileViewModel> GetMyProfileForEdit(ApplicationUser loggedInUser);

        Task<GlobalResponseData<bool>> EditMyProfilForEdit(EditMyProfileViewModel model, ApplicationUser loggedInUser);

        Task<Response> SendEmailConfirmation(string email, string confirmationLink);

        Task<Response> SendEmailResetPassword(string email, string passwordResetLink);
    }
}
