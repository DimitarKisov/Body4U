namespace Body4U.Web.Controllers
{
    using Body4U.Common;
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Services.Data.Contracts;
    using Body4U.Web.ViewModels.Article;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class ArticleController : Controller
    {
        private readonly IArticleService articleService;
        private readonly UserManager<ApplicationUser> userManager;

        public ArticleController(IArticleService articleService, UserManager<ApplicationUser> userManager)
        {
            this.articleService = articleService;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Administrator, Trainer")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Trainer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateArticleRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            var result = await articleService.Create(model, user);

            if (result.IsValid)
            {
                return RedirectToAction("Get", "Article", new { result.Data.Id });
            }

            ModelState.AddModelError(string.Empty, result.Error.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult All(int pageNumber = 1, int pageSize = 6)
        {
            var result = articleService.All(pageNumber, pageSize);

            if (result != null)
            {
                return View(result);
            }

            return View("HttpError");
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            GlobalResponseData<GetArticleResponse> result = null;
            var currentlyLoggedInUser = await userManager.GetUserAsync(this.User);

            if (currentlyLoggedInUser == null)
            {
                result = articleService.Get(id);
            }
            else
            {
                result = articleService.Get(id, currentlyLoggedInUser);
            }

            if (!result.IsValid)
            {
                if (result.Error.Message == GlobalConstants.ArticleMissing)
                {
                    ViewBag.ErrorMessage = result.Error.Message;
                    return View("NotFound");
                }
                else if (result.Error.Message == GlobalConstants.Wrong)
                {
                    ViewBag.ErrorMessage = result.Error.Message;
                    return View("HttpError");
                }
            }

            return View(result.Data);
        }

        [Authorize(Roles = "Administrator, Trainer")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentlyLoggedInUser = await userManager.GetUserAsync(User);
            var result = await articleService.Edit(id, currentlyLoggedInUser);

            if (result.Error.Message == GlobalConstants.ArticleMissing)
            {
                ViewBag.ErrorMessage = result.Error.Message;
                return View("NotFound");
            }
            else if (result.Error.Message == GlobalConstants.Wrong)
            {
                ViewBag.ErrorMessage = result.Error.Message;
                return View("HttpError");
            }

            return View(result.Data);
        }

        [Authorize(Roles = "Administrator, Trainer")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditArticleRequestModel model)
        {
            var currentlyLoggedInUser = await userManager.GetUserAsync(User);
            var result = await articleService.Edit(model, currentlyLoggedInUser);

            if (result.Error.Message == GlobalConstants.ArticleMissing)
            {
                ViewBag.ErrorMessage = result.Error.Message;
                return View("NotFound");
            }
            else if (result.Error.Message == GlobalConstants.WrongImageFormat)
            {
                ViewBag.ErrorMessage = result.Error.Message;
                return View("NotFound");
            }
            else if (result.Error.Message == GlobalConstants.Wrong)
            {
                ViewBag.ErrorMessage = result.Error.Message;
                return View("HttpError");
            }

            return RedirectToAction("MyProfile", "Account");
        }
    }
}
