using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Areas.Admin.Helpers;
using Project_sem_3.Models;

namespace Project_sem_3.Controllers
{
    public class LogonController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public LogonController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: /Admin/Logon
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("CandidateId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Admin/Logon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password, bool remember = false)
        {
            // ✅ Validate empty input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both username and password.";
                return View();
            }

            // ✅ Find candidate by username
            var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.CandidateCode == username);
            if (candidate == null)
            {
                ViewBag.Error = "Account does not exist.";
                return View();
            }

            // ✅ Check if account is active
            if (candidate.Status != 1)
            {
                ViewBag.Error = "Your account is inactive. Please contact the administrator.";
                return View();
            }

            // ✅ Compare hashed password
            var hashedInput = PasswordHelper.HashPassword(password);
            var storedHash = candidate.Pass;

            if (string.IsNullOrEmpty(storedHash) ||
                !string.Equals(hashedInput, storedHash, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "Incorrect password.";
                return View();
            }

            // ✅ Save session info
            HttpContext.Session.SetInt32("CandidateId", candidate.Id);
            HttpContext.Session.SetString("CandidateName", candidate.Fullname ?? "Unknown");

            // (Optional) Implement "Remember Me" with persistent cookie if needed

            // ✅ Redirect to Home after login
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Admin/Logon/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}

