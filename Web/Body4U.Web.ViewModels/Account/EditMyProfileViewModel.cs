namespace Body4U.Web.ViewModels.Account
{
    using Body4U.Data.Models.Enums;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EditMyProfileViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Първото име е задължително!")]
        [RegularExpression("^([A-Z][a-z]+|[А-Я][а-я]+)$", ErrorMessage = "Моля въведете валидно име.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилията е задължителна!")]
        [RegularExpression("^([A-Z][a-z]+|[А-Я][а-я]+)$", ErrorMessage = "Моля въведете валидна фамилия.")]
        public string LastName { get; set; }

        [Range(1, 100, ErrorMessage = "Въвъдете валидни години.")]
        public int? Age { get; set; }

        [RegularExpression("^([0-9]{10}|[+0-9]{13})$", ErrorMessage = "Невалиден номер.")]
        public string PhoneNumber { get; set; }

        public IFormFile ProfilePicture { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }
}
