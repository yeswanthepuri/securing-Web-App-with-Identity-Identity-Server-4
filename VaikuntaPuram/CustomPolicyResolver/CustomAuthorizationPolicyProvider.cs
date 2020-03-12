using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace VaikuntaPuram.CustomPolicyResolver
{

    public class SecurityLevelAttribute : AuthorizeAttribute
    {
        public SecurityLevelAttribute(int level)
        {
            Policy = $"{DynamicPolicy.SecurityLevel}.{level}";
        }
    }

    public static class DynamicPolicy
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
        }
        public const string SecurityLevel = "SecurityLevel";
        public const string Rank = "Rank";

    }
    public static class DynamicAuthPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            var part = policyName.Split('.');
            var type = part.First();
            var Value = part.Last();

            switch (type)
            {
                case DynamicPolicy.Rank:
                    return new AuthorizationPolicyBuilder().RequireClaim("Rank", Value).Build();
                case DynamicPolicy.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                                .AddRequirements(new SecurityLevelRequirement(Convert.ToInt32(Value)))
                                .Build();
                default:
                    return null;
            }

        }
    }

    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public int Level;
        public SecurityLevelRequirement(int level)
        {
            this.Level = level;
        }
    }
    public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        SecurityLevelRequirement requirement)
        {
            var claimsValue = Convert.ToInt32(context.User.Claims.
            FirstOrDefault(x => x.Type == DynamicPolicy.SecurityLevel)?.Value ?? "0");

            if (requirement.Level >= claimsValue)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }

    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) 
        : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var customPolicy in DynamicPolicy.Get())
            {
                if (policyName.StartsWith(customPolicy))
                {
                    var policy = DynamicAuthPolicyFactory.Create(policyName);
                    return Task.FromResult(policy);
                }
            }
            return base.GetPolicyAsync(policyName);
        }
    }
}