
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace VaikuntaPuram.Controllers
{
    public class OperationController : Controller
    {
        private readonly IAuthorizationService authorization;

        public OperationController(IAuthorizationService authorization)
        {
            this.authorization = authorization;
        }

        public async Task<IActionResult> Open()
        {
            var cookieJar = new  CookieJar() ;//get Cookie  jar from db
            var requirement = new OperationAuthorizationRequirement()
            {
                Name = CookieJarOperations.ComeNear
            };
            var result = await authorization.AuthorizeAsync(User,cookieJar, requirement);
            return View("Index");
        }
    }
    public class CookieJarAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement,CookieJar>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,CookieJar cookieJar)
        {
            if (requirement.Name == CookieJarOperations.Look)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement.Name == CookieJarOperations.ComeNear)
            {
                if (context.User.HasClaim("Friday", "Good"))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }

    public static class CookieJarOperations
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";

        public static string ComeNear = "ComeNear";
        public static string Look = "Look";

    }

    public class CookieJar
    {
        public string Name { get; set; }
    }
}
