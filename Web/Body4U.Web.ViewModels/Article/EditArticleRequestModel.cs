namespace Body4U.Web.ViewModels.Article
{
    using Body4U.Data.Models.Enums;
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public class EditArticleRequestModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заглавието е задължително.")]
        public string Title { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Съдържанието е задължително.")]
        public string Content { get; set; }

        public ArticleType ArticleType { get; set; }
    }
}
