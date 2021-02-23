﻿namespace Body4U.Web.Controllers
{
    using Body4U.Data.Models;
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

            return RedirectToAction("HttpError", "Home");
        }
    }
}
