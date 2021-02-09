namespace Body4U.Web.Controllers
{
    using Body4U.Common;
    using Body4U.Data.Models;
    using Body4U.Services.Data.Contracts;
    using Body4U.Web.ViewModels.Account;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountController(IAccountService accountService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            this.accountService = accountService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emailAlreadyExsists = await userManager.FindByEmailAsync(model.Email);
            if (emailAlreadyExsists != null)
            {
                ModelState.AddModelError(string.Empty, GlobalConstants.EmailExists);
                return View(model);
            }

            var result = await accountService.Register(model);

            if (result.IsValid)
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(result.Data);

                var confirmationLink = Url.Action(nameof(VerifyEmail), "Account",
                    new { userId = result.Data.Id, token = token }, Request.Scheme, Request.Host.ToString());

                var sendRespond = await accountService.SendEmailConfirmation(result.Data.Email, confirmationLink);

                if (sendRespond.IsSuccessStatusCode)
                {
                    return View("AfterRegister");
                }

                return View("SendMailFail");
            }

            ModelState.AddModelError(string.Empty, result.Error.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, GlobalConstants.PleaseConfirmEmail);
                    return View(model);
                }

                var result = await accountService.Login(model);

                if (result)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Невалиден email или парола.");
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);

            var result = await accountService.ChangePassword(model, user);

            if (result == false)
            {
                return View(model);
            }

            return RedirectToAction("MyProfile", "Account");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    var sendRespond = await accountService.SendEmailResetPassword(user.Email, passwordResetLink);

                    if (sendRespond.IsSuccessStatusCode)
                    {
                        return View("ForgotPasswordConfirmation");
                    }

                    return View("SendMailFail");
                }

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", GlobalConstants.InvalidPasswordResetToken);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }

                    ModelState.AddModelError("", GlobalConstants.UnssuccesfulPasswordReset);

                    return View(model);
                }

                //За да избегнем брут форс атаки е добре, да не разкриваме, че не е намерен такъв потребител
                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var result = accountService.MyProfile(user);

                return View(result);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var loggedInUser = await userManager.GetUserAsync(User);
            var result = accountService.EditMyProfile(loggedInUser);

            return View(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditMyProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loggedInUser = await userManager.GetUserAsync(User);
            var result = await accountService.EditMyProfile(model, loggedInUser);

            if (result.IsValid)
            {
                return RedirectToAction("MyProfile", "Account");
            }
            else if (result.Error.Message == GlobalConstants.WrongImageFormat)
            {
                ModelState.AddModelError(string.Empty, GlobalConstants.WrongImageFormat);
                return View(model);
            }

            ModelState.AddModelError(string.Empty, GlobalConstants.Wrong);
            return View(model);
        }

        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            return BadRequest();
        }
    }
}
