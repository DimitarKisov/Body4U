namespace Body4U.Data.Models.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum Gender
    {
        [Display(Name = "Мъж")]
        Male = 0,
        [Display(Name = "Жена")]
        Female = 1
    }
}
