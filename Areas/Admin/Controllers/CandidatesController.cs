using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CandidatesController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public CandidatesController(online_aptitude_testsContext context) {
        _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var candidate = _context.Candidates.ToListAsync();
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
    }
}
