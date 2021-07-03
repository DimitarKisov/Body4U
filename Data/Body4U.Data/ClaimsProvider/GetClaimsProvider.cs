namespace Body4U.Data.ClaimsProvider
{
    using Body4U.Common;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    public class GetClaimsProvider : IGetClaimsProvider
    {
        public string UserId { get; private set; }

        public string Email { get; private set; }

        public bool? IsAdmin { get; private set; }

        public bool? IsTrainer { get; private set; }

        public GetClaimsProvider(IHttpContextAccessor accessor)
        {
            var claims = accessor?.HttpContext?.User?.Claims;

            if (claims != null)
            {
                UserId = claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                Email = claims.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                IsAdmin = IsUserAdmin(claims);
                IsTrainer = IsUserTrainer(claims);
            }
        }

        private bool? IsUserAdmin(IEnumerable<Claim> claims)
        {
            if (claims.Count() > 0)
            {
                var roles = claims.Where(x => x.Type == ClaimTypes.Role);
                return roles.Any(x => x.Value == GlobalConstants.AdministratorRoleName);
            }

            return null;
        }

        private bool? IsUserTrainer(IEnumerable<Claim> claims)
        {
            if (claims.Count() > 0)
            {
                var roles = claims.Where(x => x.Type == ClaimTypes.Role);
                return roles.Any(x => x.Value == GlobalConstants.TrainerRoleName);
            }

            return null;
        }
    }
}
