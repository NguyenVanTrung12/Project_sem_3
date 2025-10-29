using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using X.PagedList.Extensions;

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

        // GET: Admin/Transfers
        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;
            var transfers = _context.Transfers
                .Include(t => t.Candidate)
                .OrderByDescending(t => t.TransferDate)
                .ToPagedList(page, pageSize);

            return View(transfers);
        }

        // GET: Admin/Transfers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var transfer = await _context.Transfers
                .Include(t => t.Candidate)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transfer == null)
                return NotFound();

            return View(transfer);
        }

        // GET: Admin/Transfers/Create
        public IActionResult Create()
        {
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Fullname");
            return View();
        }

        // POST: Admin/Transfers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CandidateId,TransferDate,FromStage,ToStage")] Transfer transfer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transfer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Fullname", transfer.CandidateId);
            return View(transfer);
        }

        // GET: Admin/Transfers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var transfer = await _context.Transfers.FindAsync(id);
            if (transfer == null)
                return NotFound();

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Fullname", transfer.CandidateId);
            return View(transfer);
        }

        // POST: Admin/Transfers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CandidateId,TransferDate,FromStage,ToStage")] Transfer transfer)
        {
            if (id != transfer.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transfer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Transfers.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Fullname", transfer.CandidateId);
            return View(transfer);
        }

        // GET: Admin/Transfers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var transfer = await _context.Transfers
                .Include(t => t.Candidate)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transfer == null)
                return NotFound();

            return View(transfer);
        }

        // POST: Admin/Transfers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transfer = await _context.Transfers.FindAsync(id);
            if (transfer != null)
            {
                _context.Transfers.Remove(transfer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
