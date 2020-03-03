using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VaikuntaPuram.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }
        public IActionResult Authenticate()
        {
            var grandmanClaimes = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Yeswanth"),
                new Claim(ClaimTypes.Email,"epuri.yeshu@gmail.com"),
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
    }

}