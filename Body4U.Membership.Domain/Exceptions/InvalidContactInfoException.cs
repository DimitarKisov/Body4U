using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Domain.Exceptions
{
    public class InvalidContactInfoException : BaseDomainException
    {
        public InvalidContactInfoException()
        {
        }

        public InvalidContactInfoException(string message)
            : base(message)
        {
        }
    }
}
