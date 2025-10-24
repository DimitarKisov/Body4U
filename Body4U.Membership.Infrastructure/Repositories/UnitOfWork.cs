using Body4U.Membership.Infrastructure.Persistence;
using Body4U.SharedKernel.Domain;
using MediatR;

namespace Body4U.Membership.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MembershipDbContext _dbContext;
        private readonly IPublisher _publisher;

        public UnitOfWork(
            MembershipDbContext dbContext,
            IPublisher publisher)
        {
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch domain events before saving
            var domainEvents = _dbContext.ChangeTracker
                .Entries<Entity<object>>()
                .Select(x => x.Entity)
                .Where(x => x.DomainEvents.Any())
                .SelectMany(x =>
                {
                    var events = x.DomainEvents.ToList();
                    x.ClearDomainEvents();
                    return events;
                })
                .ToList();

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            // Publish events after successful save
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }

            return result;
        }
    }
}