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

        public DbSet<Trainer> Trainers { get; set; }

        public DbSet<TrainerImage> TrainerImages { get; set; }

        public DbSet<TrainerVideo> TrainerVideos { get; set; }

        public DbSet<Article> Articles { get; set; }

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

            #region Таблици
            builder.Entity<Trainer>()
                .HasOne(t => t.ApplicationUser)
                .WithOne(a => a.Trainer)
                .HasForeignKey<Trainer>(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrainerImage>()
                .HasOne(ti => ti.Trainer)
                .WithMany(t => t.TrainerImages)
                .HasForeignKey(ti => ti.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrainerVideo>()
                .HasOne(ti => ti.Trainer)
                .WithMany(t => t.TrainerVideos)
                .HasForeignKey(ti => ti.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Article>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(au => au.Articles)
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }

        // Applies configurations
        private void ConfigureUserIdentityRelations(ModelBuilder builder)
             => builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

    }
}
