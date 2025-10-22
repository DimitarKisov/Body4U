using MediatR;

namespace Body4U.SharedKernel.Domain
{
    public interface IDomainEvent : INotification
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }
}
