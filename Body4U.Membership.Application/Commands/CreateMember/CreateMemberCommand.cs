namespace Body4U.Membership.Application.Commands.CreateMember
{
    using Body4U.Membership.Application.Repositories;
    using Body4U.Membership.Domain.Enumerations;
    using Body4U.Membership.Domain.Models;
    using Body4U.SharedKernel.Domain;
    using MediatR;

    public class CreateMemberCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int MembershipLevelId { get; set; }

        internal class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, Guid>
        {
            private readonly IMemberRepository _memberRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateMemberCommandHandler(
                IMemberRepository memberRepository,
                IUnitOfWork unitOfWork)
            {
                _memberRepository = memberRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Guid> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
            {
                var memberExists = await _memberRepository.MemberExistsAsync(request.Email, cancellationToken);
                if (memberExists)
                {
                    throw new InvalidOperationException($"Member with email {request.Email} already exists.");
                }

                var membershipLevel = Enumeration.FromValue<MembershipLevel>(request.MembershipLevelId);

                var member = Member.Create(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber,
                    membershipLevel);

                _memberRepository.Add(member);
                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {

                    throw;
                }

                return member.Id;
            }
        }
    }
}
