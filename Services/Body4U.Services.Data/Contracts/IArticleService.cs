namespace Body4U.Services.Data.Contracts
{
    using Body4U.Data.ClaimsProvider;
    using Body4U.Data.Models;
    using Body4U.Data.Models.Helper;
    using Body4U.Web.ViewModels.Article;
    using cloudscribe.Pagination.Models;
    using System.Threading.Tasks;

    public interface IArticleService
    {
        Task<GlobalResponseData<Article>> Create(CreateArticleRequest model, ApplicationUser user);

        Task<PagedResult<GetAllArticlesViewModel>> All(int pageNumber, int pageSize);

        Task<GlobalResponseData<GetArticleResponse>> Get(int id, ApplicationUser currentlyLoggedInUser = null);

        Task<GlobalResponseData<EditArticleViewModel>> Edit(int id, IGetClaimsProvider claimsProvider);

        Task<GlobalResponse> Edit(EditArticleRequestModel model, IGetClaimsProvider claimsProvider);

        Task<GlobalResponse> Delete(int id, IGetClaimsProvider claimsProvider);
    }
}
