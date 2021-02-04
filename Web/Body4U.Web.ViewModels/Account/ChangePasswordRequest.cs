namespace Body4U.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Текущата парола е задължнителна!")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Новата парола е задължителна!")]
        [StringLength(20, ErrorMessage = "Новата парола трябва да бъде между {2} и {1} символа дълга!", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат")]
        public string ConfirmPassword { get; set; }
    }
}
