using Body4U.Membership.Application.Repositories;
using Body4U.Membership.Domain.Models;
using Body4U.Membership.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Body4U.Membership.Infrastructure.Repositories
{
    internal class MemberRepository : IMemberRepository
    {
        private readonly MembershipDbContext _dbContext;

        public MemberRepository(MembershipDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Member member)
        {
            _dbContext.Members.AddAsync(member);
        }

        public async Task<bool> MemberExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Members.AnyAsync(x => x.ContactInfo.Email == email, cancellationToken);
        }
    }
}
