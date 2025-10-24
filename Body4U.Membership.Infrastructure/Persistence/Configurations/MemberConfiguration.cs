using Body4U.Membership.Domain.Enumerations;
using Body4U.Membership.Domain.Models;
using Body4U.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Body4U.Membership.Infrastructure.Persistence.Configurations
{
    internal class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("Members");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.DateJoined)
                .IsRequired();

            builder.Property(x => x.LoyaltyPoints)
                .IsRequired();

            builder.Property(x => x.CurrentMonthBookings)
                .IsRequired();

            // Complex Type за ContactInfo
            builder.OwnsOne(x => x.ContactInfo, contact =>
            {
                contact.Property(y => y.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(200)
                    .IsRequired();

                contact.Property(y => y.PhoneNumber)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            // Enumeration conversions
            builder.Property(x => x.MembershipLevel)
                .HasConversion(
                    level => level.Id,
                    id => Enumeration.FromValue<MembershipLevel>(id))
                .IsRequired();

            builder.Property(x => x.MembershipStatus)
                .HasConversion(
                    status => status.Id,
                    id => Enumeration.FromValue<MembershipStatus>(id))
                .IsRequired();

            // Ignore domain events
            builder.Ignore(x => x.DomainEvents);

            // Index
            builder.HasIndex(x => x.ContactInfo.Email)
                .IsUnique();
        }
    }
}
