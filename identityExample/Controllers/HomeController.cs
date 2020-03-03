using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using identityExample.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace identityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> UserManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailService EmailService;

        public HomeController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IEmailService emailService)
        {
            this.UserManager = userManager;
            this.signInManager = signInManager;
            this.EmailService = emailService;
        }
        public IActionResult EmailVerification() { 
            return View();
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
        [HttpPost]
        public async Task<IActionResult> LogIn(string userName, string password)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                var signInResult = await signInManager.PasswordSignInAsync(user, password, false, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                //SignIn
            }
            return View();
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string userName, string password)
        {
            var user = new IdentityUser { UserName = userName, Email = "" };
            var resut = await UserManager.CreateAsync(user, password);
            if (resut.Succeeded)
            {
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(VerifyEmail), "Home", new { userID = user.Id, code = code }, Request.Scheme, Request.Host.ToString());
                //await EmailService.SendAsync("epuri.yeshu@gmail.com", "Email Verify", $"<a>{link}</a>");
                TempData["emaillink"]=link;
                return RedirectToAction("EmailVerification");
            }
            return this.View();
        }


        public async Task<IActionResult> VerifyEmail(string userID, string code)
        {
            var user = await UserManager.FindByIdAsync(userID);
            if (user == null) return BadRequest();
            var code_find = await UserManager.ConfirmEmailAsync(user, code);

            if (code_find.Succeeded)
            {
                return View();
            }
            return BadRequest();
        }
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Register()
        {

            return View();
        }
    }

}