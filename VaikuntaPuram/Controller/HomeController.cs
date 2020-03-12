using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaikuntaPuram.CustomPolicyResolver;

namespace VaikuntaPuram.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }
        [Authorize(Policy = "Claims.DOB")]
        public IActionResult SecurePolicy()
        {
            return View("Secure");
        }
        //Roles are legicy code

        // [Authorize(Roles = "Admin")]

        [SecurityLevel(5)]
        public IActionResult LowLevel()
        {
            return View("Secure");
        }
         [SecurityLevel(10)]
        public IActionResult HighLevel()
        {
            return View("Secure");
        }
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var grandmanClaimes = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Yeswanth"),
                new Claim(ClaimTypes.Email,"epuri.yeshu@gmail.com"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim(ClaimTypes.DateOfBirth,"1/1/2020"),
                new Claim(DynamicPolicy.SecurityLevel,"7"),
                new Claim("Grand","Bob")
            };
            var licenseClaimes = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Krithvika"),
                new Claim(ClaimTypes.Email,"epuri.Krithvika@gmail.com"),
                new Claim("LicensetoDrive","A+")
            };
            var grandmaaIdentity = new ClaimsIdentity(grandmanClaimes, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaimes, "Government");

            var userPrinciple = new ClaimsPrincipal(new[] { grandmaaIdentity, licenseIdentity });
            HttpContext.SignInAsync(userPrinciple);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff(
            [FromServices] IAuthorizationService authorizationService)
        {
            var builder = new AuthorizationPolicyBuilder("Schema");

            var customPolicy =builder.RequireClaim("Hello").Build();
           var result= await authorizationService.AuthorizeAsync(HttpContext.User,customPolicy);

            if(result.Succeeded)
            {
                
            }
            return View("Index");
        }
    }

}