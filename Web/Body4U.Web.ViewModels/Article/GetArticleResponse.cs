namespace Body4U.Web.ViewModels.Article
{
    using Ganss.XSS;
    using System.Collections.Generic;

    public class GetArticleResponse
    {
        private HtmlSanitizer sanitizer;
        public GetArticleResponse()
        {
            //RecentArticles = new List<GetRecentArticlesViewModel>();
            sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Add("iframe");
        }
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string ContentToView => sanitizer.Sanitize(Content);

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

        //public Dictionary<string, int> ArticleTypesCount { get; set; }

        //public List<GetRecentArticlesViewModel> RecentArticles { get; set; }
    }
}
