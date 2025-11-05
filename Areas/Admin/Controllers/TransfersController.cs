using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Threading.Tasks;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransfersController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public TransfersController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.Transfers
                .Include(t => t.Candidate)
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();

            return View(list);
        }
    }
}
