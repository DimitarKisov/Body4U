﻿namespace Body4U.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Body4U.Data.Models.Enums;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
        }

        [Required]
        [MinLength(3), MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3), MaxLength(50)]
        public string LastName { get; set; }

        public string FullName => this.FirstName + " " + this.LastName;

        [Range(5, 100, ErrorMessage = "Въведете валидни години.")]
        public int? Age { get; set; }

        public byte[] ProfilePicture { get; set; }

        [Required]
        public Gender Sex { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public bool IsDisabled { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
    }
}
