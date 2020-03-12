using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace VaikuntaPuram.AuthorizationRequirement
{
    public class CutomRequiredClaim : IAuthorizationRequirement
    {
        public string ClaimType { get; }

        public CutomRequiredClaim(string claimType)
        {
            this.ClaimType = claimType;
        }

    }
    public class CustomRequiredClaimHandler : AuthorizationHandler<CutomRequiredClaim>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CutomRequiredClaim requirement)
        {
            var hasclaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
            if (hasclaim)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }

    public static class AuthorizationPolicyBuilderExtension
    {
        public static AuthorizationPolicyBuilder RequestCustomClaim(this AuthorizationPolicyBuilder builder, string claim)
        {
            builder.AddRequirements(new CutomRequiredClaim(claim));
            return builder;
        }
    }
}