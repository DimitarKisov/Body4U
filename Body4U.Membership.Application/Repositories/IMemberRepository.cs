using Body4U.Membership.Domain.Models;
using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Application.Repositories
{
    public interface IMemberRepository : IRepository<Member>
    {
        Task<bool> MemberExistsAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(Member member, CancellationToken cancellationToken = default);
    }
}
