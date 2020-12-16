using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using registration_simple_webapp.Models;
using registration_simple_webapp.Models.DAO;
using registration_simple_webapp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace registration_simple_webapp.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationContext db;
        public const string Blocked = "blocked";
        public const string Unblocked = "unblocked";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            db = context;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            return Content(User.Identity.Name);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Enter()
        {
            ViewBag.Items = db.Users.ToArray();
            var model = new CheckboxListModel { Selected = new List<bool>() };
            for (int i = 0; i < db.Users.Count(); i++)
                model.Selected.Add(false);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Block(CheckboxListModel model)
        {
            string current = User.Identity.Name;
            if (current != null)
                return await ChangeStatus(current, model, Blocked);
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> Unblock(CheckboxListModel model)
        {
            string current = User.Identity.Name;
            if (current != null)
                return await ChangeStatus(current, model, Unblocked);
            return RedirectToAction("Login", "Account");
        }


        public async Task<IActionResult> ChangeStatus(string current, CheckboxListModel model, string status)
        {
            bool blockYourself = ChangeUsersStatus(model, db.Users.ToList(), current, status);
            await db.SaveChangesAsync();
            if (blockYourself || db.Users.FirstOrDefault(u => u.Email == current).Status == Blocked)
                return await Logout();
            return RedirectToAction("Enter", "Home");
        }

        private bool ChangeUsersStatus(CheckboxListModel model, List<User> users, string current, string status)
        {
            for (int i = 0; i < model.Selected.Count; i++)
            {
                if (!ChangeUserStatus(model.Selected[i], users[i], current, status))
                    return true;
            }
            return false;
        }


        private bool ChangeUserStatus(bool isSelected, User user, string current, string status)
        {
            if (isSelected)
            {
                user.Status = status;
                if (user.Email == current && status == Blocked)
                    return false;
            }
            return true;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CheckboxListModel model)
        {
            string current = User.Identity.Name;
            if (current != null)
            {
                if(db.Users.FirstOrDefault(u => u.Email == current).Status == Blocked)
                    return await Logout();
                return await DeleteUsersAsync(current, model, await db.Users.ToListAsync());
            }
            return RedirectToAction("Login", "Account");
        }

        private async Task<IActionResult> DeleteUsersAsync(string current, CheckboxListModel model, List<User> users)
        {
            bool shouldQuit = false;
            DeleteUsers(current, model, users, ref shouldQuit);
            await db.SaveChangesAsync();
            if (shouldQuit)
                return await Logout();
            return RedirectToAction("Enter", "Home");
        }

        private void DeleteUsers(string current, CheckboxListModel model, List<User> users, ref bool shouldQuit)
        {
            for (int i = 0; i < model.Selected.Count; i++)
            {
                if(!DeleteUser(model.Selected[i], users[i], current))
                {
                    shouldQuit = true;
                }
            }
        }

        private bool DeleteUser(bool isSelected, User user, string current)
        {
            if (isSelected)
            {
                db.Users.Remove(user);
                if (user.Email == current)
                    return false;
            }
            return true;
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
