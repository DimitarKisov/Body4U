namespace Body4U.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Body4U.Data.Models.Enums;

    public class Article
    {
        public int Id { get; set; }

        [Required]
        [MinLength(30)]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public byte[] Image { get; set; }

        [Required]
        [MinLength(50)]
        public string Content { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public ArticleType ArticleType { get; set; }
    }
}
