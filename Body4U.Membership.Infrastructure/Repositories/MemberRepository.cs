using Body4U.Membership.Application.DTOs;
using Body4U.Membership.Application.Repositories;
using Body4U.Membership.Domain.Models;
using Body4U.Membership.Infrastructure.Persistence;
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

        public async Task<MemberDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Members
                .Select(x => new MemberDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.ContactInfo.Email,
                    PhoneNumber = x.ContactInfo.PhoneNumber,
                    MembershipLevel = x.MembershipLevel.Name,
                    MembershipStatus = x.MembershipStatus.Name,
                    DateJoined = x.DateJoined,
                    ExpirationDate = x.ExpirationDate,
                    LoyaltyPoints = x.LoyaltyPoints,
                    CurrentMonthBookings = x.CurrentMonthBookings,
                    RemainingBookings = x.MembershipLevel.MonthlyBookingLimit - x.CurrentMonthBookings
                })
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> MemberExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Members.AnyAsync(x => x.ContactInfo.Email == email, cancellationToken);
        }
    }
}
