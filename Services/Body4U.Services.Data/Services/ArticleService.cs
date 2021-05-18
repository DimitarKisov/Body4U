namespace Body4U.Services.Data.Services
{
    using Body4U.Common;
    using Body4U.Data;
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Services.Data.Contracts;
    using Body4U.Web.ViewModels.Article;
    using cloudscribe.Pagination.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class ArticleService : IArticleService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public ArticleService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<GlobalResponseData<Article>> Create(CreateArticleRequest model, ApplicationUser user)
        {
            try
            {
                var trainer = await dbContext.Trainers.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

                if ((trainer != null && trainer.IsReadyToWrite))
                {

                    if (dbContext.Articles.Any(x => x.Title == model.Title))
                    {
                        return GlobalResponseData<Article>.BadResponse(GlobalConstants.ArticleTitleExsists);
                    }
                    if (model.Image.ContentType != "image/jpeg" && model.Image.ContentType != "image/png")
                    {
                        return GlobalResponseData<Article>.BadResponse(GlobalConstants.WrongImageFormat);
                    }

                    var article = new Article()
                    {
                        Title = model.Title.Trim(),
                        Content = model.Content.Trim(),
                        ApplicationUserId = user.Id,
                        ArticleType = model.ArticleType,
                        DatePosted = DateTime.Now
                    };

                    //Проверяваме размера на снимката. Ако е по-голяма от 0, значи има избран файл
                    if (model.Image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            //Отваряме стрийм и записваме съдържанието на снимката в него
                            model.Image.CopyTo(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                //if (img.Width < 730 || img.Height < 548)
                                //{
                                //    return ResponseData<Article>.BadResponse(GlobalConstants.WrongImageWidthHeight);
                                //}

                                using (var ms = new MemoryStream())
                                {
                                    img.Save(ms, img.RawFormat);
                                    article.Image = ms.ToArray();
                                }
                            }
                        }
                    }

                    await dbContext.Articles.AddAsync(article);
                    await dbContext.SaveChangesAsync();

                    return GlobalResponseData<Article>.CorrectResponse(article);
                }
                else
                {
                    return GlobalResponseData<Article>.BadResponse(GlobalConstants.NotReadyToWriteArticle);
                }
            }
            catch (Exception)
            {
                return GlobalResponseData<Article>.BadResponse(GlobalConstants.Wrong);
            }
        }

        public PagedResult<GetAllArticlesViewModel> All(int pageNumber, int pageSize)
        {
            try
            {
                var articles = dbContext
                .Articles
                .Include(x => x.ApplicationUser)
                .OrderByDescending(x => x.DatePosted)
                .Select(a => new GetAllArticlesViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    Image = Convert.ToBase64String(a.Image),
                    AuthorFullName = a.ApplicationUser.FullName,
                    DatePosted = a.DatePosted.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    MonthNamePosted = a.DatePosted.ToString("MMM", CultureInfo.InvariantCulture),
                    DateNumberPosted = a.DatePosted.ToString("dd", CultureInfo.InvariantCulture),
                    ArticleType = a.ArticleType.ToString()
                })
                .Skip((pageSize * pageNumber) - pageSize)
                .Take(pageSize)
                .AsQueryable();

                var result = new PagedResult<GetAllArticlesViewModel>
                {
                    Data = articles.AsNoTracking().ToList(),
                    TotalItems = dbContext.Articles.Count(),
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GlobalResponseData<GetArticleResponse>> Get(int id, ApplicationUser currentlyLoggedInUser = null)
        {
            try
            {
                var article = await dbContext.Articles.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == id);
                if (article == null)
                {
                    return GlobalResponseData<GetArticleResponse>.BadResponse(GlobalConstants.ArticleMissing);
                }

                var trainer = dbContext.Trainers.FirstOrDefault(x => x.ApplicationUserId == article.ApplicationUserId);

                var articles = dbContext.Articles.AsQueryable();
                
                var result = new GetArticleResponse
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    Image = Convert.ToBase64String(article.Image),
                    DatePosted = article.DatePosted.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    AuthorName = article.ApplicationUser.FullName,
                    ArticleType = article.ArticleType.ToString(),
                    AuthorId = article.ApplicationUserId,
                    ShortBio = trainer?.ShortBio ?? "",
                    AuthorProfilePicture = Convert.ToBase64String(article.ApplicationUser.ProfilePicture),
                    AuthorFacebook = trainer?.FacebookUrl ?? "",
                    AuthorInstagram = trainer?.InstagramUrl ?? "",
                    AuthorYoutubeChannel = trainer?.YoutubeChannelUrl ?? "",
                    //LoggedUserName = currentlyLoggedInUser != null ? $"{currentlyLoggedInUser.FirstName} {currentlyLoggedInUser.LastName}" : "",
                    //IsInFavourites = currentlyLoggedInUser != null ? dbContext.Favourites.Any(x => x.ArticleId == article.Id && x.ApplicationUserId == currentlyLoggedInUser.Id) : false
                };

                var dic = new Dictionary<string, int>();

                //Типовете статии и техния брой
                foreach (var type in articles)
                {
                    var articleType = (type.ArticleType).ToString();

                    if (!dic.ContainsKey(articleType))
                    {
                        dic[articleType] = 0;
                    }
                    dic[articleType]++;
                }
                result.ArticleTypesCount = dic;

                //var articlesCount = articles.Count() > 4 ? 4 : articles.Count();
                //var recentArticles = articles
                //    .Select(x => new GetRecentArticlesViewModel
                //    {
                //        Id = x.Id,
                //        Image = Convert.ToBase64String(x.Image),
                //        Title = x.Title,
                //        DatePosted = x.DatePosted.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                //    })
                //    .Take(articlesCount);

                //result.RecentArticles.AddRange(recentArticles);

                return GlobalResponseData<GetArticleResponse>.CorrectResponse(result);
            }
            catch (Exception ex)
            {
                return GlobalResponseData<GetArticleResponse>.BadResponse(GlobalConstants.Wrong);
            }
        }

        public async Task<GlobalResponseData<EditArticleViewModel>> Edit(int id, ApplicationUser currentlyLoggedInUser)
        {
            try
            {
                var article = dbContext.Articles.Find(id);

                if (article == null)
                {
                    return GlobalResponseData<EditArticleViewModel>.BadResponse(GlobalConstants.ArticleMissing);
                }

                if (article.ApplicationUserId != currentlyLoggedInUser.Id)
                {
                    if (!await userManager.IsInRoleAsync(currentlyLoggedInUser, GlobalConstants.AdministratorRoleName) && currentlyLoggedInUser.Email != configuration.GetSection("SeedInfo")["UserName"])
                    {
                        //TODO Какво ще се връща ако не е нито треньора, нито администратора (не знам как би станало изобщо тва)
                        return null;
                    }
                }

                var result = new EditArticleViewModel()
                {
                    Title = article.Title,
                    Content = article.Content,
                    ArticleType = article.ArticleType,
                    Image = Convert.ToBase64String(article.Image),
                };

                return GlobalResponseData<EditArticleViewModel>.CorrectResponse(result);
            }
            catch (Exception)
            {
                return GlobalResponseData<EditArticleViewModel>.BadResponse(GlobalConstants.Wrong);
            }
        }

        public async Task<GlobalResponse> Edit(EditArticleRequestModel model, ApplicationUser currentlyLoggedInUser)
        {
            try
            {
                var article = dbContext.Articles.Find(model.Id);

                if (article == null)
                {
                    return GlobalResponse.BadResponse(GlobalConstants.ArticleMissing);
                }

                if (article.ApplicationUserId != currentlyLoggedInUser.Id)
                {
                    if (!await userManager.IsInRoleAsync(currentlyLoggedInUser, GlobalConstants.AdministratorRoleName) && currentlyLoggedInUser.Email != configuration.GetSection("SeedInfo")["UserName"])
                    {
                        //TODO Какво ще се връща ако не е нито треньора, нито администратора (не знам как би станало изобщо тва)
                        return null;
                    }
                }

                if (model.Image.ContentType != "image/jpeg" && model.Image.ContentType != "image/png")
                {
                    return GlobalResponseData<Article>.BadResponse(GlobalConstants.WrongImageFormat);
                }

                article.Title = model.Title.Trim();
                article.Content = model.Content.Trim();
                article.ArticleType = model.ArticleType;

                if (model.Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        //Отваряме стрийм и записваме съдържанието на снимката в него
                        model.Image.CopyTo(memoryStream);
                        using (var img = Image.FromStream(memoryStream))
                        {
                            //if (img.Width < 730 || img.Height < 548)
                            //{
                            //    return ResponseData<Article>.BadResponse(GlobalConstants.WrongImageWidthHeight);
                            //}

                            using (var ms = new MemoryStream())
                            {
                                img.Save(ms, img.RawFormat);
                                article.Image = ms.ToArray();
                            }
                        }
                    }
                }

                await dbContext.SaveChangesAsync();

                return GlobalResponse.CorrectResponse();
            }
            catch (Exception)
            {
                return GlobalResponse.BadResponse(GlobalConstants.Wrong);
            }
        }
    }
}
