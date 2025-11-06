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
            var managers = _context.Managers
            .Include(e => e.Role)
            .Where(e => e.Role.RoleName == "Role_Managers")
            .Take(1)
            .ToList();
            return View(managers);
        }
    }
}
