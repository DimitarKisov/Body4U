namespace Body4U.Services.Data.Contracts
{
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Web.ViewModels.Article;
    using cloudscribe.Pagination.Models;
    using System.Threading.Tasks;

    public interface IArticleService
    {
        Task<GlobalResponseData<Article>> Create(CreateArticleRequest model, ApplicationUser user);

        PagedResult<GetAllArticlesViewModel> All(int pageNumber, int pageSize);

        GlobalResponseData<GetArticleResponse> Get(int id, ApplicationUser currentlyLoggedInUser = null);

        Task<GlobalResponseData<EditArticleViewModel>> Edit(int id, ApplicationUser currentlyLoggedInUser);

        Task<GlobalResponse> Edit(EditArticleRequestModel model, ApplicationUser currentlyLoggedInUser);
    }
}
