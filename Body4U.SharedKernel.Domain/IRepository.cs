namespace Body4U.SharedKernel.Domain
{
    public interface IRepository<TEntity>
        where TEntity : IAggregateRoot
    {
    }
}
