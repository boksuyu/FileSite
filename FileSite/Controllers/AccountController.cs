using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FileSite.Data;
using FileSite.Data.ViewModels;
using FileSite.Models;
using FileSite.Services.EmailServicing;
using Serilog;

namespace FileSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailService _emailService;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                 EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            var pageReloaded = new LoginViewModel();
            return View(pageReloaded);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user != null)
            {
                bool passwordCheck = await _userManager.CheckPasswordAsync(user, login.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, login.Password, true, false);
                    if (result.Succeeded) { return RedirectToAction("Index", "Home"); }

                }
                TempData["Error"] = "Wrong Info";
                return View(login);
            }
            TempData["Error"] = "Wrong Info";
            return View(login);
        }
        //------------------------------------//

        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            { return View(register); }

            AppUser? user = await _userManager.FindByEmailAsync(register.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "Email In Use";
                return View(register);
            }

            AppUser newUser = new AppUser()
            {
                Email = register.EmailAddress,
                UserName = register.EmailAddress,
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, register.Password);

            if (newUserResponse.Succeeded) { await _userManager.AddToRoleAsync(newUser, UserRoles.User); }
            return RedirectToAction("Login");

        }
        //------------------------------------//

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Account/PasswordRecovery")]
        public IActionResult PasswordRecovery(string to)
        {   
            EmailVM request = new EmailVM() {to = to};
            request.subject = "subject"; 
            request.body = "body";
            if (_userManager.FindByEmailAsync(request.to) == null)
                { return RedirectToAction("Login");}
            _emailService.SendEmail(request);
            return RedirectToAction("Login");
        }

        [HttpGet("Account/NewPassword/{link}")]
        public IActionResult NewPassword(string link)
        {
            string? email = _emailService.CheckAvailableLink(link);
            if (email == null)
            { return BadRequest(); }
            ViewBag.to = email;
            return View();
        }

        [HttpPost("Account/NewPassword/{link}")]
        public async Task<IActionResult> NewPassword(AccountRecoveryVM request, string link)
        {   request.Email = _emailService.CheckAvailableLink(link);
            if (request.Email == null || request.NewPassword == null)
                { return BadRequest(); }
            AppUser? user = _userManager.FindByEmailAsync(request.Email).Result;
            if (user==null)
                { return BadRequest(); }

            string reset =await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, reset, request.NewPassword);
            _emailService.CleanUsedLink(link);
            return RedirectToAction("Index", "Home");
        }
    }
}
