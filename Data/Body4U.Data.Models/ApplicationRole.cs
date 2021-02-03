// ReSharper disable VirtualMemberCallInConstructor
namespace Body4U.Data.Models
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
            : this(null)
        {
        }

        public ApplicationRole(string name)
            : base(name)
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
