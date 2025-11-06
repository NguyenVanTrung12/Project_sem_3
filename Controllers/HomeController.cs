using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Diagnostics;

namespace Project_sem_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, online_aptitude_testsContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Banner = _context.Banners
                .Where(b => b.Active == 1)
                .ToList();

            ViewBag.Manage = _context.Managers
                 .Include(m => m.Role)
                 .Where(m => m.Status == 1 && m.Role.RoleName == "Role_Managers")
                 .ToList();

            ViewBag.Blog = _context.Blogs
                  .Where(m => m.Status == 1)
                  .ToList();

            return View();
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
    }
}
