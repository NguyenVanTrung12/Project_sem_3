using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;

namespace Project_sem_3.Controllers
{
    public class AboutController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        private readonly ILogger<AboutController> _logger;
        public AboutController(online_aptitude_testsContext context, ILogger<AboutController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [Route("/about")]
        public IActionResult Index()
        {
            var manager = _context.Managers
                 .Include(m => m.Role)
                 .Where(m => m.Status == 1 && m.Role.RoleName == "Role_Managers")
                 .ToList();
            return View(manager);
        }
    }
}
