namespace Body4U.Services.Data.Services
{
    using Body4U.Common;
    using Body4U.Data;
    using Body4U.Data.ClaimsProvider;
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Services.Data.Contracts;
    using Body4U.Web.ViewModels.Article;
    using cloudscribe.Pagination.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Serilog;
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

        public ArticleService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<GlobalResponseData<Article>> Create(CreateArticleRequest model, ApplicationUser user)
        {
            try
            {
                var trainer = await dbContext.Trainers.FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

                if (trainer != null && trainer.IsReadyToWrite)
                {
                    if (await dbContext.Articles.AnyAsync(x => x.Title == model.Title))
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
            catch (Exception ex)
            {
                Log.Error(ex, "ArticleService: Create POST");
                return GlobalResponseData<Article>.BadResponse(GlobalConstants.Wrong);
            }
        }

        public async Task<PagedResult<GetAllArticlesViewModel>> All(int pageNumber, int pageSize)
        {
            try
            {
                var articles = dbContext
                .Articles
                .OrderByDescending(x => x.DatePosted)
                .Select(a => new GetAllArticlesViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    Image = Convert.ToBase64String(a.Image),
                    Author = a.ApplicationUser.FullName,
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
                    Data = await articles.AsNoTracking().ToListAsync(),
                    TotalItems = dbContext.Articles.Count(),
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ArticleService: All");
                return null;
            }
        }

        public async Task<GlobalResponseData<GetArticleResponse>> Get(int id, ApplicationUser currentlyLoggedInUser = null)
        {
            try
            {
                var article = await dbContext.Articles
                    .Select(x => new
                    {
                        x.Id,
                        x.Title,
                        x.Content,
                        x.Image,
                        x.DatePosted,
                        x.ArticleType,
                        x.ApplicationUserId,
                        Author = x.ApplicationUser.FirstName + " " + x.ApplicationUser.LastName,
                        AuthorProfilePicture = x.ApplicationUser.ProfilePicture
                    })
                    .FirstOrDefaultAsync(x => x.Id == id);

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
                    AuthorName = article.Author,
                    ArticleType = article.ArticleType.ToString(),
                    AuthorId = article.ApplicationUserId,
                    ShortBio = trainer?.ShortBio ?? "",
                    AuthorProfilePicture = Convert.ToBase64String(article.AuthorProfilePicture),
                    AuthorFacebook = trainer?.FacebookUrl ?? "",
                    AuthorInstagram = trainer?.InstagramUrl ?? "",
                    AuthorYoutubeChannel = trainer?.YoutubeChannelUrl ?? ""
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

                return GlobalResponseData<GetArticleResponse>.CorrectResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ArticleService: Get");
                return GlobalResponseData<GetArticleResponse>.BadResponse(GlobalConstants.Wrong);
            }
        }

        public async Task<GlobalResponseData<EditArticleViewModel>> Edit(int id, IGetClaimsProvider claimsProvider)
        {
            try
            {
                var article = await dbContext.Articles.FindAsync(id);

                if (article == null)
                {
                    return GlobalResponseData<EditArticleViewModel>.BadResponse(GlobalConstants.ArticleMissing);
                }

                if (article.ApplicationUserId != claimsProvider.UserId && claimsProvider.IsTrainer.HasValue && claimsProvider.IsAdmin.HasValue)
                {
                    return GlobalResponseData<EditArticleViewModel>.BadResponse(GlobalConstants.WrongRights);
                }
                else if (article.ApplicationUserId != claimsProvider.UserId && claimsProvider.IsTrainer.HasValue && claimsProvider.IsAdmin.HasValue)
                {
                    return GlobalResponseData<EditArticleViewModel>.BadResponse(GlobalConstants.NotFound);
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
            catch (Exception ex)
            {
                Log.Error(ex, "ArticleService: Edit GET");
                return GlobalResponseData<EditArticleViewModel>.BadResponse(GlobalConstants.Wrong);
            }
        }

        public async Task<GlobalResponse> Edit(EditArticleRequestModel model, IGetClaimsProvider claimsProvider)
        {
            try
            {
                var article = dbContext.Articles.Find(model.Id);

                if (article == null)
                {
                    return GlobalResponse.BadResponse(GlobalConstants.ArticleMissing);
                }

                if (article.ApplicationUserId != claimsProvider.UserId && claimsProvider.IsTrainer.HasValue && claimsProvider.IsAdmin.HasValue)
                {
                    return GlobalResponse.BadResponse(GlobalConstants.WrongRights);
                }
                else if (article.ApplicationUserId != claimsProvider.UserId && claimsProvider.IsTrainer.HasValue && claimsProvider.IsAdmin.HasValue)
                {
                    return GlobalResponse.BadResponse(GlobalConstants.NotFound);
                }

                if (model.Image.ContentType != "image/jpeg" && model.Image.ContentType != "image/png")
                {
                    return GlobalResponse.BadResponse(GlobalConstants.WrongImageFormat);
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
            catch (Exception ex)
            {
                Log.Error(ex, "ArticleService: Edit POST");
                return GlobalResponse.BadResponse(GlobalConstants.Wrong);
            }
        }

        public async Task<GlobalResponse> Delete(int id, IGetClaimsProvider claimsProvider)
        {
            try
            {
                var article = await dbContext.Articles.FindAsync(id);

                if (article == null)
                {
                    return GlobalResponse.BadResponse(GlobalConstants.ArticleMissing);
                }

                if (article.ApplicationUserId != claimsProvider.UserId && claimsProvider.IsTrainer.HasValue && claimsProvider.IsAdmin.HasValue)
                {
                    return GlobalResponse.BadResponse(GlobalConstants.WrongRights);
                }
                else if (article.ApplicationUserId != claimsProvider.UserId && claimsProvider.IsTrainer.HasValue && claimsProvider.IsAdmin.HasValue)
                {
                    return GlobalResponse.BadResponse(GlobalConstants.NotFound);
                }

                dbContext.Articles.Remove(article);
                await dbContext.SaveChangesAsync();

                return GlobalResponse.CorrectResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ArticleService: Delete");
                return GlobalResponse.BadResponse(GlobalConstants.Wrong);
            }
        }
    }
}
