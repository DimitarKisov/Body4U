using Body4U.SharedKernel.Domain;

namespace Body4U.Membership.Domain.Enumerations
{
    public class MembershipLevel : Enumeration
    {
        public static readonly MembershipLevel Basic = new(1, "Basic", 8, 0, 100m);
        public static readonly MembershipLevel Premium = new(2, "Premium", 12, 5, 150m);
        public static readonly MembershipLevel Elite = new(3, "Elite", 20, 10, 200m);

        public MembershipLevel(int id, string name, int monthlyBookingLimit, int priorityBonus, decimal monthlyFee)
            : base(id, name)
        {
        }

        public int MonthlyBookingLimit { get; private set; }
        public int PriorityBonus { get; private set; }
        public decimal MonthlyFee { get; private set; }

        public bool CanBookMoreClasses(int currentBookings)
        {
            return currentBookings < MonthlyBookingLimit;
        }

        public int GetWaitlistPriority()
        {
            return Id * 10 + PriorityBonus;
        }
    }
}
