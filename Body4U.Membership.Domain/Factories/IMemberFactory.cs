using Body4U.Membership.Domain.Enumerations;
using Body4U.Membership.Domain.Models;
using Body4U.Membership.Domain.ValueObjects;
using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Domain.Factories
{
    public interface IMemberFactory : IFactory<Member>
    {
        IMemberFactory WithFirstName(string firstName);
        IMemberFactory WithLastName(string lastName);
        IMemberFactory WithContactInfo(ContactInfo contactInfo);
        IMemberFactory WithMembershipLevel(MembershipLevel membershipLevel);
    }
}
