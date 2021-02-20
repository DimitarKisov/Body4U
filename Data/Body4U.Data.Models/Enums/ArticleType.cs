namespace Body4U.Data.Models.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum ArticleType
    {
        [Display(Name = "Хранене")]
        Eating = 0,
        [Display(Name = "Рецепти")]
        Recepies = 1,
        [Display(Name = "Тренировки")]
        Training = 2,
        [Display(Name = "Здраве")]
        Health = 3,
        [Display(Name = "Суплементи")]
        Supplements = 4
    }
}
