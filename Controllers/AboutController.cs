using Microsoft.AspNetCore.Mvc;
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
            var manager = _context.Managers.ToList();
            return View(manager);
        }
    }
}
