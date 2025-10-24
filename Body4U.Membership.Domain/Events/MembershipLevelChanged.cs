using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Domain.Events
{
    public record MembershipLevelChanged(Guid Id, DateTime OccurredOn, Guid MemberId, int OldLevelId, int NewLevelId)
        : DomainEvent(Id, OccurredOn)
    {
        public MembershipLevelChanged(Guid memberId, int oldLevelId, int newLevelId)
            : this(Guid.NewGuid(), DateTime.UtcNow, memberId, oldLevelId, newLevelId)
        {
        }
    }
}