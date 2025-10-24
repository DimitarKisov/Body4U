namespace Body4U.Membership.Application.Commands.CreateMember
{
    using FluentValidation;

    using static Domain.Constants.ModelConstants.Member;

    internal class CreateMemberValidator : AbstractValidator<CreateMemberCommand>
    {
        public CreateMemberValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(MaxNameLength).WithMessage($"First name cannot exceed {MaxNameLength} characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(MaxNameLength).WithMessage($"Last name cannot exceed {MaxNameLength} characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format");

            RuleFor(x => x.MembershipLevelId)
                .InclusiveBetween(1, 3).WithMessage("Invalid membership level ID (must be 1-3)");
        }
    }
}
