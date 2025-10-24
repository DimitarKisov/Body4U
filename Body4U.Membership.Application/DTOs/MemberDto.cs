namespace Body4U.Membership.Application.DTOs
{
    public class MemberDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MembershipLevel { get; set; }
        public string MembershipStatus { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int LoyaltyPoints { get; set; }
        public int CurrentMonthBookings { get; set; }
        public int RemainingBookings { get; set; }
    }
}
