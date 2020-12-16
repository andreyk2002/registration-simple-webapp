
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using registration_simple_webapp.Models;
using registration_simple_webapp.Models.DAO;
using registration_simple_webapp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace registration_simple_webapp.Controllers
{
    public class AccountController : Controller
    {

        private ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
                return await CheckLoginAsync(model);
            return View(model);
        }

        private async Task<IActionResult> CheckLoginAsync(LoginModel model)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email &&
               u.Password == model.Password);
            if (user != null)
                return await EnterAccountAsync(user, model);
            ModelState.AddModelError("", "Invalid Login or(and)password");
            return View(model);
        }

        private async Task<RedirectToActionResult> EnterAccountAsync(User user, LoginModel model)
        {
            user.LastLoginDate = DateTime.Now;
            user.Status = HomeController.Unblocked;
            await db.SaveChangesAsync();
            await Authenticate(model.Email);
            return RedirectToAction("Enter", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid) 
                return await FindOrAddUserAsync(model);
            return View(model);
        }

        private async Task<IActionResult> FindOrAddUserAsync(RegisterModel model)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
                return await AddUserAsync(model);
            else
                ModelState.AddModelError("", "Account with email " + model.Email + " already exists");
            return View(model);
        }

        private async Task<RedirectToActionResult> AddUserAsync(RegisterModel model)
        {
            db.Users.Add(new User(model.Name, model.Email, model.Password));
            await db.SaveChangesAsync();
            await Authenticate(model.Email);
            return RedirectToAction("Enter", "Home");
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, userName) };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(id));
        }
              
    }
}
