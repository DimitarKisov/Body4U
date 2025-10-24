using Body4U.Membership.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Body4U.Membership.Infrastructure.Persistence
{
    public class MembershipDbContext : DbContext
    {
        public MembershipDbContext(DbContextOptions<MembershipDbContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MembershipDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
