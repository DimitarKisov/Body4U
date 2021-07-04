namespace Body4U.Web.ViewModels.Account
{
    using System.Net;
    using System.Text.RegularExpressions;

    public class MyArticlesViewModel
    {
        public int Id { get; set; }

        public string Image { get; set; }

        public string Title { get; set; }

        public string ShortTitle => this.Title.Length > 30 ? Title.Substring(0, 27) + "..." : Title;

        public string Content { get; set; }

        public string ShortContent
        {
            get
            {
                var content = WebUtility.HtmlDecode(Regex.Replace(Content, @"<[^>]+>", string.Empty));
                return content.Length > 131 ? content.Substring(0, 131) + "..." : content;
            }

        }

        public string DatePosted { get; set; }

        public string ApplicationUserId { get; set; }
    }
}
