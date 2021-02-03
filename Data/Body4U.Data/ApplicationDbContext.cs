namespace Body4U.Data
{
    using Body4U.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            this.ConfigureUserIdentityRelations(builder);

            #region Смяна на имена на таблици
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "IdentityUsers");
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "IdentityRoles");
            });
            #endregion
        }

        // Applies configurations
        private void ConfigureUserIdentityRelations(ModelBuilder builder)
             => builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

    }
}
