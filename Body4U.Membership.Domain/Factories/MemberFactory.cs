using Body4U.Membership.Domain.Enumerations;
using Body4U.Membership.Domain.Models;
using Body4U.Membership.Domain.ValueObjects;

namespace Body4U.Membership.Domain.Factories
{
    internal class MemberFactory : IMemberFactory
    {
        private string _firstName;
        private string _lastName;
        private ContactInfo _contactInfo;
        private MembershipLevel _membershipLevel;

        public Member Build()
        {
            return new Member(Guid.NewGuid(), _firstName, _lastName, _contactInfo, _membershipLevel);
        }

        public IMemberFactory WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public IMemberFactory WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public IMemberFactory WithContactInfo(ContactInfo contactInfo)
        {
            _contactInfo = contactInfo;
            return this;
        }

        public IMemberFactory WithMembershipLevel(MembershipLevel membershipLevel)
        {
            _membershipLevel = membershipLevel;
            return this;
        }
    }
}
