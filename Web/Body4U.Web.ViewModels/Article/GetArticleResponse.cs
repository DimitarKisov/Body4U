using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Body4U.Web.ViewModels.Article
{
    public class GetArticleResponse
    {
        public GetArticleResponse()
        {
            RecentArticles = new List<GetRecentArticlesViewModel>();
        }
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string ContentToView => new HtmlSanitizer().Sanitize(Content);

        public string Image { get; set; }

        public string DatePosted { get; set; }

        public string AuthorName { get; set; }

        public string ArticleType { get; set; }

        public string AuthorId { get; set; }

        public string ShortBio { get; set; }

        public string AuthorProfilePicture { get; set; }

        public string AuthorFacebook { get; set; }

        public string AuthorInstagram { get; set; }

        public string AuthorYoutubeChannel { get; set; }

        public int CommentsCount { get; set; }

        public bool IsInFavourites { get; set; }

        public string LoggedUserName { get; set; }

        public Dictionary<string, int> ArticleTypesCount { get; set; }

        public List<GetRecentArticlesViewModel> RecentArticles { get; set; }
    }
}
