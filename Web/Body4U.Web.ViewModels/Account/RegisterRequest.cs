namespace Body4U.Web.ViewModels.Account
{
    using Body4U.Data.Models.Enums;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RegisterRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Невалиден email адрес.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [RegularExpression("^([0-9]{10}|[+0-9]{13})$", ErrorMessage = "Невалиден номер.")]
        [Display(Name = "Телефонен номер")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Първото име е задължително!")]
        [RegularExpression("^([A-Z][a-z]+|[А-Я][а-я]+)$", ErrorMessage = "Моля въведете валидно име.")]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилията е задължнителна!")]
        [RegularExpression("^([A-Z][a-z]+|[А-Я][а-я]+)$", ErrorMessage = "Моля въведете валидна фамилия.")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Range(1, 100, ErrorMessage = "Въвъдете валидни години.")]
        [Display(Name = "Години")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Моля изберете пол.")]
        public Gender Gender { get; set; }

        public IFormFile ProfilePicture { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Паролата трябва да бъде поне {2} символа дълга.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Потвърди парола")]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; }
    }
}
