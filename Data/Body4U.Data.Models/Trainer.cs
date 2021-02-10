namespace Body4U.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class Trainer
    {
        public Trainer()
        {
            Id = Guid.NewGuid().ToString();
            TrainerImages = new HashSet<TrainerImage>();
            TrainerVideos = new HashSet<TrainerVideo>();
        }

        public string Id { get; set; }

        [MinLength(200, ErrorMessage = "Твърде кратка биография.")]
        public string Bio { get; set; }

        [MaxLength(200, ErrorMessage = "Твърде много текст за кратка биография.")]
        public string ShortBio { get; set; }

        [RegularExpression(@"(?:(?:http|https):\/\/)?(?:www.)?facebook.com\/(?:(?:\w)*#!\/)?(?:pages\/)?(?:[?\w\-]*\/)?(?:profile.php\?id=(?=\d.*))?([\w.\-]*)?", ErrorMessage = "Невалиден Facebook профил.")]
        public string FacebookUrl { get; set; }

        [RegularExpression(@"(?:(?:http|https):\/\/)?(?:www\.)?(?:instagram\.com|instagr\.am)\/([A-Za-z0-9-_\.]+)", ErrorMessage = "Невалиден Instagram профил.")]
        public string InstagramUrl { get; set; }

        [RegularExpression(@"((http|https):\/\/|)(www\.|)youtube\.com\/(channel\/|user\/)[a-zA-Z0-9\-]{1,}", ErrorMessage = "Невалиден YouTube канал.")]
        public string YoutubeChannelUrl { get; set; }

        [DefaultValue(false)]
        public bool IsReadyToVisualize { get; set; }

        [DefaultValue(false)]
        public bool IsReadyToWrite { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<TrainerImage> TrainerImages { get; set; }

        public virtual ICollection<TrainerVideo> TrainerVideos { get; set; }
    }
}
