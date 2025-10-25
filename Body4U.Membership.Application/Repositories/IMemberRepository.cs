using Body4U.Membership.Application.DTOs;
using Body4U.Membership.Domain.Models;
using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Application.Repositories
{
    public interface IMemberRepository : IRepository<Member>
    {
        void Add(Member member);
        Task<MemberDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> MemberExistsAsync(string email, CancellationToken cancellationToken = default);
    }
}
