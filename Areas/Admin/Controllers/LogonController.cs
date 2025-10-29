
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Areas.Admin.Helpers;
using Project_sem_3.Models;
using System.Security.Claims;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LogonController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public LogonController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: /Admin/Logon
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("ManagerId") != null)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        // POST: /Admin/Logon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password, bool remember = false)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both username and password.";
                return View();
            }
            // 1️⃣ Check if username exists
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Username == username);
            if (manager == null)
            {
                ViewBag.Error = "Username does not exist.";
                return View();
            }

            // 2️⃣ Check if password field is null
            if (string.IsNullOrWhiteSpace(manager.PasswordHash))
            {
                ViewBag.Error = "This account does not have a password set.";
                return View();
            }

            // 3️⃣ Compare hashed passwords
            var hashedInput = PasswordHelper.HashPassword(password);
            if (!string.Equals(hashedInput, manager.PasswordHash, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "Incorrect password.";
                return View();
            }

            // 4️⃣ Create claims for authentication
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, manager.Username ?? "Unknown"),
        new Claim("Fullname", manager.Fullname ?? "Administrator"),
        new Claim(ClaimTypes.Role, "Admin")
    };

            var claimsIdentity = new ClaimsIdentity(claims, "AdminCookie");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = remember
            };

            // 5️⃣ Sign in using cookie authentication
            await HttpContext.SignInAsync("AdminCookie", new ClaimsPrincipal(claimsIdentity), authProperties);

            // 6️⃣ Store information in session
            if (manager.Id > 0)
            {
                HttpContext.Session.SetInt32("ManagerId", manager.Id);
            }
            HttpContext.Session.SetString("ManagerName", manager.Fullname ?? "Admin");

            // ✅ Verify if session was stored successfully
            var test = HttpContext.Session.GetInt32("ManagerId");
            if (test == null)
            {
                Console.WriteLine("⚠️ Warning: Session data was not saved correctly!");
            }

            // 7️⃣ Redirect to Admin dashboard
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Admin/Logon/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            HttpContext.Session.Clear();
            return Redirect("https://localhost:7105/");
        }
    }
}

