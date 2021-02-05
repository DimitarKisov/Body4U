namespace Body4U.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
