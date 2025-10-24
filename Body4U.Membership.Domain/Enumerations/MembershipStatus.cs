using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Domain.Enumerations
{
    public class MembershipStatus : Enumeration
    {
        public static readonly MembershipStatus Active = new(1, "Active");
        public static readonly MembershipStatus Suspended = new(2, "Suspended");
        public static readonly MembershipStatus Expired = new(3, "Expired");
        public static readonly MembershipStatus Cancelled = new(4, "Cancelled");

        public MembershipStatus(int id, string name) : base(id, name)
        {
        }

        public bool CanBook()
        {
            return this == Active;
        }

        public bool CanRenew()
        {
            return this == Expired || this == Suspended;
        }
    }
}
