namespace Body4U.Data.ClaimsProvider
{
    public interface IGetClaimsProvider
    {
        string UserId { get; }

        string Email { get; }

        bool? IsAdmin { get; }

        bool? IsTrainer { get; }
    }
}
