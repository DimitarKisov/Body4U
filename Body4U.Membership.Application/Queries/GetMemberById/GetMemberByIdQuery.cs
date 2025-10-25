namespace Body4U.Membership.Application.Queries.GetMemberById
{
    using Body4U.Membership.Application.DTOs;
    using Body4U.Membership.Application.Repositories;
    using MediatR;

    public class GetMemberByIdQuery : IRequest<MemberDto>
    {
        public Guid Id { get; set; }

        internal class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto>
        {
            private readonly IMemberRepository _memberRepository;

            public GetMemberByIdQueryHandler(IMemberRepository memberRepository)
            {
                _memberRepository = memberRepository;
            }
            public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
            {
                var member = await _memberRepository.GetByIdAsync(request.Id, cancellationToken);

                if (member == null)
                {
                    throw new InvalidOperationException($"Member with ID {request.Id} not found");
                }

                return member;
            }
        }
    }
}
