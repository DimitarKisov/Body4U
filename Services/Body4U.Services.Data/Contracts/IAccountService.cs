﻿namespace Body4U.Services.Data.Contracts
{
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Web.ViewModels.Account;
    using System.Threading.Tasks;

    public interface IAccountService
    {
        Task<ResponseData<ApplicationUser>> Register(RegisterRequest model);

        Task<bool> Login(LoginRequest model);

        Task<bool> ChangePassword(ChangePasswordRequest model, ApplicationUser user);

        Task<SendGrid.Response> SendEmailConfirmation(string email, string confirmationLink);
    }
}
