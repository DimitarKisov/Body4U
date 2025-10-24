using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Domain.Exceptions
{
    public class InvalidMemberException : BaseDomainException
    {
        public InvalidMemberException()
        {
        }

        public InvalidMemberException(string message)
            : base(message)
        {
        }
    }
}
