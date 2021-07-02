namespace Body4U.Web.ViewModels.Article
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    public class GetAllArticlesViewModel
    {
        public int Id { get; set; }

        public string Image { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string ApplicationUserId { get; set; }

        public string Author { get; set; }

        public string DatePosted { get; set; }

        public string MonthNamePosted { get; set; }

        public string DateNumberPosted { get; set; }

        public string ArticleType { get; set; }

        public string SubstringedContent
        {
            get
            {
                var content = WebUtility.HtmlDecode(Regex.Replace(Content, @"<[^>]+>", string.Empty));
                return content.Length > 200 ? content.Substring(0, 200) + "..." : content;
            }
        }
    }
}
