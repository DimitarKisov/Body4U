namespace Body4U.Web.Controllers
{
    using Body4U.Common;
    using Body4U.Data.Models;
    using Body4U.Services.Data.Contracts;
    using Body4U.Web.ViewModels.Account;
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
