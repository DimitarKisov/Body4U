namespace Body4U.Web.ViewModels.Article
{
    using Body4U.Data.Models.Enums;

    public class EditArticleViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Content { get; set; }

        public ArticleType ArticleType { get; set; }
    }
}
