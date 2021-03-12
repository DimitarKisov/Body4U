namespace Body4U.Web.ViewModels.Article
{
    public class GetRecentArticlesViewModel
    {
        public int Id { get; set; }

        public string Image { get; set; }

        public string Title { get; set; }

        public string SubstringedTitle => Title.Length > 25 ? Title.Substring(0, 25) + "..." : Title;

        public string DatePosted { get; set; }
    }
}
