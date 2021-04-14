namespace Body4U.Web.ViewModels.Article
{
    using Body4U.Data.Models.Enums;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreateArticleRequest
    {
        [Required(ErrorMessage = "Заглавието е задължително!")]
        [MinLength(3)]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Снимката е задължителна!")]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Съдържанието е задължнително!")]
        [MinLength(700)]
        public string Content { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required(ErrorMessage = "Типът на статията е задължителен!")]
        public ArticleType ArticleType { get; set; }
    }
}
