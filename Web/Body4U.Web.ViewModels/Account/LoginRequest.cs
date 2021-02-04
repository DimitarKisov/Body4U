namespace Body4U.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class LoginRequest
    {
        [Required(ErrorMessage = "Полето за email е задължнително.")]
        [EmailAddress(ErrorMessage = "Невалиден email адрес.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Полето за парола е задължително.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
