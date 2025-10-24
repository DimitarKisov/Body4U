using Body4U.Membership.Domain.Exceptions;
using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Domain.ValueObjects
{
    public class ContactInfo : ValueObject
    {
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        private ContactInfo(string email, string phoneNumber)
        {
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public static ContactInfo Create(string email, string phoneNumber)
        {
            Validate(email, phoneNumber);
            return new ContactInfo(email, phoneNumber);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Email;
            yield return PhoneNumber;
        }

        private static void Validate(string email, string phoneNumber)
        {
            Guard.AgainstEmptyString<InvalidContactInfoException>(email, nameof(email));
            Guard.AgainstNotContainingSpecialChars<InvalidContactInfoException>(email, "Invalid email format", "@");
            Guard.AgainstEmptyString<InvalidContactInfoException>(phoneNumber, nameof(phoneNumber));
        }
    }
}
