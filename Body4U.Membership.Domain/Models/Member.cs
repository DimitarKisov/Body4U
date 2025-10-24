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

        public static Member Create(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            MembershipLevel membershipLevel)
        {
            var contactInfo = ContactInfo.Create(email, phoneNumber);
            Validate(firstName, lastName, contactInfo);

            var memberId = Guid.NewGuid();
            var member = new Member(memberId, firstName, lastName, contactInfo, membershipLevel);

            member.AddDomainEvent(new MemberCreated(memberId, email, membershipLevel.Id));

            return member;
        }

        public void ChangeMembershipLevel(MembershipLevel newLevel)
        {
            if (MembershipLevel == newLevel)
            {
                return;
            }

            var oldLevelId = MembershipLevel.Id;

            MembershipLevel = newLevel;

            AddDomainEvent(new MembershipLevelChanged(Id, oldLevelId, newLevel.Id));
        }

        public bool CanBookClass()
        {
            if (!MembershipStatus.CanBook())
            {
                return false;
            }

            if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow)
            {
                MembershipStatus = MembershipStatus.Expired;
                return false;
            }

            return MembershipLevel.CanBookMoreClasses(CurrentMonthBookings);
        }

        public void RecordBooking()
        {
            if (!CanBookClass())
            {
                throw new InvalidMemberException("Member cannot book more classes.");
            }

            CurrentMonthBookings++;
            LoyaltyPoints += 10;
        }

        public void ResetMonthlyBookings()
        {
            CurrentMonthBookings = 0;
        }

        public void Suspend(string reason)
        {
            if (MembershipStatus == MembershipStatus.Cancelled)
            {
                throw new InvalidMemberException("Cannot suspend cancelled membership");
            }

            MembershipStatus = MembershipStatus.Suspended;
        }

        public void Reactivate()
        {
            if (!MembershipStatus.CanRenew())
            {
                throw new InvalidMemberException("Cannot reactivate this membership");
            }

            MembershipStatus = MembershipStatus.Active;
            ExpirationDate = DateTime.UtcNow.AddMonths(1);
        }

        public void Cancel()
        {
            MembershipStatus = MembershipStatus.Cancelled;
        }

        public int GetWaitlistPriority()
        {
            return MembershipLevel.GetWaitlistPriority() + (LoyaltyPoints / 100);
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        private static void Validate(string firstName, string lastName, ContactInfo contactInfo)
        {
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
            ValidateContactInfo(contactInfo);
        }

        private static void ValidateFirstName(string firstName)
        {
            Guard.AgainstEmptyString<InvalidMemberException>(firstName, nameof(firstName));
            Guard.ForStringLength<InvalidMemberException>(firstName, MinNameLength, MaxNameLength, nameof(firstName));
        }

        private static void ValidateLastName(string lastName)
        {
            Guard.AgainstEmptyString<InvalidMemberException>(lastName, nameof(lastName));
            Guard.ForStringLength<InvalidMemberException>(lastName, MinNameLength, MaxNameLength, nameof(lastName));
        }

        private static void ValidateContactInfo(ContactInfo contactInfo)
        {
            Guard.AgainstEmptyString<InvalidMemberException>(contactInfo.Email, nameof(contactInfo.Email));
            Guard.AgainstNotContainingSpecialChars<InvalidMemberException>(contactInfo.Email, "Invalid email format", "@");
            Guard.AgainstEmptyString<InvalidMemberException>(contactInfo.PhoneNumber, nameof(contactInfo.PhoneNumber));
        }
    }
}
