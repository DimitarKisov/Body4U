namespace Body4U.Web.Areas.Administration.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GetAllUsersViewModel
    {
        public GetAllUsersViewModel()
        {
            this.Roles = new HashSet<GetAllUserRolesViewModel>();
        }
        public string Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public ICollection<GetAllUserRolesViewModel> Roles { get; set; }
    }
}
