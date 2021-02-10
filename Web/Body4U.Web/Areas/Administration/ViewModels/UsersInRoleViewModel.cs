namespace Body4U.Web.Areas.Administration.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class UsersInRoleViewModel
    {
        public UsersInRoleViewModel()
        {
            Users = new HashSet<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Името на ролята е задължително.")]
        public string RoleName { get; set; }

        public ICollection<string> Users { get; set; }
    }
}
