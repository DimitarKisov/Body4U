namespace Body4U.Services.Data.Services
{
    using Body4U.Common;
    using Body4U.Data;
    using Body4U.Data.Models;
    using Body4U.Data.Models.Enums;
    using Body4U.Data.Models.Helper;
    using Body4U.Services.Data.Contracts;
    using Body4U.Web.ViewModels.Article;
    using cloudscribe.Pagination.Models;
    using LazZiya.ImageResize;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
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

        public ArticleService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<ResponseData<Article>> Create(CreateArticleRequest model, ApplicationUser user)
        {
            try
            {
                var trainer = dbContext.Trainers.FirstOrDefault(x => x.ApplicationUserId == user.Id);

                if ((trainer != null && trainer.IsReadyToWrite) || await userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName))
                {

                    if (dbContext.Articles.Any(x => x.Title == model.Title))
                    {
                        return ResponseData<Article>.BadResponse(GlobalConstants.ArticleTitleExsists);
                    }
                    if (model.Image.ContentType != "image/jpeg" && model.Image.ContentType != "image/png")
                    {
                        return ResponseData<Article>.BadResponse(GlobalConstants.WrongImageFormat);
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
                                if (img.Width < 180 || img.Height < 250)
                                {
                                    return ResponseData<Article>.BadResponse(GlobalConstants.WrongImageWidthHeight);
                                }

                                using (var ms = new MemoryStream())
                                {
                                    img.Save(ms, img.RawFormat);
                                    article.Image = ms.ToArray();
                                }
                            }
                        }
                    }

                    dbContext.Articles.Add(article);
                    dbContext.SaveChanges();

                    return ResponseData<Article>.CorrectResponse(article);
                }
                else
                {
                    return ResponseData<Article>.BadResponse(GlobalConstants.NotReadyToWriteArticle);
                }
            }
            catch (Exception)
            {
                return ResponseData<Article>.BadResponse(GlobalConstants.Wrong);
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

        public ResponseData<GetArticleResponse> Get(int id, ApplicationUser currentlyLoggedInUser = null)
        {
            try
            {
                var article = dbContext.Articles.Include(x => x.ApplicationUser).FirstOrDefault(x => x.Id == id);
                if (article == null)
                {
                    return ResponseData<GetArticleResponse>.BadResponse(GlobalConstants.ArticleIdMissing);
                }

                var trainer = dbContext.Trainers.FirstOrDefault(x => x.ApplicationUserId == article.ApplicationUserId);

                var articles = dbContext.Articles.ToList();
                
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
                    ShortBio = trainer.ShortBio ?? "",
                    AuthorProfilePicture = Convert.ToBase64String(article.ApplicationUser.ProfilePicture),
                    AuthorFacebook = trainer.FacebookUrl,
                    AuthorInstagram = trainer.InstagramUrl,
                    AuthorYoutubeChannel = trainer.YoutubeChannelUrl,
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

                var articlesCount = articles.Count() > 4 ? articles.Count() : 4;
                var recentArticles = articles
                    .Select(x => new GetRecentArticlesViewModel
                    {
                        Id = x.Id,
                        Image = Convert.ToBase64String(x.Image),
                        Title = x.Title,
                        DatePosted = x.DatePosted.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)
                    })
                    .Take(articlesCount)
                    .ToList();

                result.RecentArticles.AddRange(recentArticles);

                return ResponseData<GetArticleResponse>.CorrectResponse(result);
            }
            catch (Exception ex)
            {
                return ResponseData<GetArticleResponse>.BadResponse(GlobalConstants.Wrong);
            }
        }
    }
}
