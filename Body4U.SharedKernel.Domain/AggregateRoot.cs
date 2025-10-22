namespace Body4U.SharedKernel.Domain
{
    public abstract class AggregateRoot<TId> : Entity<TId>
        where TId : notnull
    {
        protected AggregateRoot(TId id) : base(id) { }
        protected AggregateRoot() { } // За EF Core
    }
}
