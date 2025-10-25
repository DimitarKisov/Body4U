namespace Body4U.Membership.Application.Queries.GetMemberById
{
    using FluentValidation;

    internal class GetMemberByIdValidator : AbstractValidator<GetMemberByIdQuery>
    {
        public GetMemberByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Member ID cannot be empty");
        }
    }
}
