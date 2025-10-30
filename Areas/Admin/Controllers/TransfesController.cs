using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransfesController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public TransfesController(online_aptitude_testsContext context)
        {
            _context = context;
        }
       
        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;
            var transfe = _context.Transfers.Include(e => e.Candidate).OrderByDescending(e => e.Id).ToPagedList(page, pageSize);
            return View(transfe);
        }


        public async Task<IActionResult> Details(int id)
        {
            var transfe = await _context.Transfers
                .Include(t => t.Candidate)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (transfe == null)
            {
                TempData["Error"] = "No transfer found to display details.";
                return RedirectToAction(nameof(Index));
            }

            return View(transfe);
        }



        public IActionResult Create()
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transfer transfer)
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            if (!ModelState.IsValid)
            {
                return View(transfer);
            }
            try
            {
                _context.Transfers.Add(transfer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Add successfuly";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while adding the transfer.");
                return View(transfer);
            }
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            var transfe = await _context.Transfers.FirstOrDefaultAsync(e => e.Id == id);
            if (transfe == null)
            {
                TempData["Error"] = "No categories found to edit.";
                return RedirectToAction(nameof(Index));
            }

            return View(transfe);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Transfer transfe)
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            if (!ModelState.IsValid)
            {
                return View(transfe);
            }
            try
            {
                _context.Transfers.Update(transfe);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Edit successfuly";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating:" + ex.Message);
                return View(transfe);
            }
        }

       
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var transfe = await _context.Transfers.FindAsync(id);
            if (transfe == null)
            {
                TempData["Error"] = "No transfe found to delete!";
                return RedirectToAction(nameof(Index));
            }

            _context.Transfers.Remove(transfe);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Delete successfuly";
            return RedirectToAction(nameof(Index));
        }

    }
}

