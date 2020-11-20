using FilmCatalog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FilmCatalog.Controllers
{
    public class AccountController : Controller
    {
        private readonly FilmCatalogContext dbContext;
        public AccountController(FilmCatalogContext context)
        {
            dbContext = context;
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
            {
                User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Email);
                    return Redirect("~/Home/Index");
                }
                ModelState.AddModelError("", "Не правильно введен логин или пароль");
            }
            return View(model);
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
            {
                User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    dbContext.Users.Add(new User { Id = Guid.NewGuid(), Email = model.Email, Name = model.Name, Password = model.Password });
                    await dbContext.SaveChangesAsync();
                    await Authenticate(model.Email);
                    return Redirect("~/Home/Index");
                }
                else
                    ModelState.AddModelError("", "Пользователь уже существует");
            }
            return View(model);
        }

        public async Task<IActionResult> ValidateEmail(string email)
        {
            User user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Json(true);
            }
            else
                return Json(false);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
