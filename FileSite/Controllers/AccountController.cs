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
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            var PageReloaded = new LoginViewModel();
            return View(PageReloaded);
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

            AppUser user = await _userManager.FindByEmailAsync(register.EmailAddress);
            if (user != null)
            {
                TempData["Eroor"] = "Email In Use";
                return View(register);
            }

            AppUser newUser = new AppUser()
            {
                Email = register.EmailAddress,
                UserName = register.EmailAddress,
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, register.Password);
            if (newUserResponse.Succeeded) { await _userManager.AddToRoleAsync(newUser, UserRoles.User); }
            return RedirectToAction("Index", "Home");

        }
        //------------------------------------//

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Account/PasswordRecovery/{to}")]
        public JsonResult PasswordRecovery(string to)
        {   
            EmailVM request = new EmailVM() {to = to};
           request.subject = "subject"; 
           request.body = "body";
            _emailService.SendEmail(request);
            Log.Information(to);
            return Json("success");
        }
    }
}
