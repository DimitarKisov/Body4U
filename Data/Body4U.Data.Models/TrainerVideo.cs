namespace Body4U.Data.Models
{
    public class TrainerVideo
    {
        public int Id { get; set; }

        public string VideoUrl { get; set; }

        public string TrainerId { get; set; }

        public virtual Trainer Trainer { get; set; }
    }
}
