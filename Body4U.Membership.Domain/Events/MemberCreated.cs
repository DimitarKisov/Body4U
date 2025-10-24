using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Domain.Events
{
    public record MemberCreated(Guid Id, DateTime OccurredOn, Guid MemberId, string Email, int MembershipLevelId)
        : DomainEvent(Id, OccurredOn)
    {
        public MemberCreated(Guid memberId, string email, int membershipLevelId)
            : this(Guid.NewGuid(), DateTime.UtcNow, memberId, email, membershipLevelId)
        {
        }
    }
}
