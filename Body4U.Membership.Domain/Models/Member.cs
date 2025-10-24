using Body4U.Membership.Domain.Enumerations;
using Body4U.Membership.Domain.Events;
using Body4U.Membership.Domain.Exceptions;
using Body4U.Membership.Domain.ValueObjects;
using Body4U.SharedKernel.Domain;

using static Body4U.Membership.Domain.Constants.ModelConstants.Member;

namespace Body4U.Membership.Domain.Models
{
    public class Member : Entity<Guid>, IAggregateRoot
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public ContactInfo ContactInfo { get; private set; }
        public MembershipLevel MembershipLevel { get; private set; }
        public MembershipStatus MembershipStatus { get; private set; }
        public DateTime DateJoined { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public int LoyaltyPoints { get; private set; }
        public int CurrentMonthBookings { get; private set; }

        internal Member(Guid id, string firstName, string lastName, ContactInfo contactInfo, MembershipLevel membershipLevel)
            : base(id)
        {
            Validate(firstName, lastName, contactInfo);

            FirstName = firstName;
            LastName = lastName;
            ContactInfo = contactInfo;
            MembershipLevel = membershipLevel;
            DateJoined = DateTime.UtcNow;
            ExpirationDate = DateTime.UtcNow.AddMonths(1);
            LoyaltyPoints = 0;
            CurrentMonthBookings = 0;
        }

        private Member() { }

        public void ChangeMembershipLevel(MembershipLevel newLevel)
        {
            if (MembershipLevel == newLevel)
            {
                return;
            }

            var oldLevelId = MembershipLevel.Id;

            MembershipLevel = newLevel;
        }

        private void Validate(string firstName, string lastName, ContactInfo contactInfo)
        {
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
            ValidateContactInfo(contactInfo);
        }

        private void ValidateFirstName(string firstName)
        {
            Guard.AgainstEmptyString<InvalidMemberException>(firstName, nameof(firstName));
            Guard.ForStringLength<InvalidMemberException>(firstName, MinNameLength, MaxNameLength, nameof(firstName));
        }

        private void ValidateLastName(string lastName)
        {
            Guard.AgainstEmptyString<InvalidMemberException>(lastName, nameof(lastName));
            Guard.ForStringLength<InvalidMemberException>(lastName, MinNameLength, MaxNameLength, nameof(lastName));
        }

        private void ValidateContactInfo(ContactInfo contactInfo)
        {
            Guard.AgainstEmptyString<InvalidMemberException>(contactInfo.Email, nameof(contactInfo.Email));
            Guard.AgainstNotContainingSpecialChars<InvalidMemberException>(contactInfo.Email, "Invalid email format", "@");
            Guard.AgainstEmptyString<InvalidMemberException>(contactInfo.PhoneNumber, nameof(contactInfo.PhoneNumber));
        }
    }
}
