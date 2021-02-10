namespace Body4U.Data.Models
{
    public class TrainerImage
    {
        public int Id { get; set; }

        public byte[] Image { get; set; }

        public string TrainerId { get; set; }

        public virtual Trainer Trainer { get; set; }
    }
}
