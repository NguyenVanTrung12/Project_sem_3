using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagerController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public ManagerController(online_aptitude_testsContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var managers = await _context.Managers
                                        .Include(m => m.Role)
                                        .ToListAsync();

            return View(managers);
        }
    }
}
