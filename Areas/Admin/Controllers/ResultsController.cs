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
    public class ResultsController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public ResultsController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: Admin/Results
        public IActionResult Index(int page = 1, int? status = null)
        {
            int pageSize = 5;
            var results = _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .Include(r => r.Type)
                .AsQueryable();

            if (status.HasValue)
            {
                results = results.Where(r => r.Status == status.Value);
            }

            var paged = results.OrderByDescending(r => r.Id).ToPagedList(page, pageSize);
            ViewBag.Status = status;
            return View(paged);
        }

        // GET: Admin/Results/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var result = await _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .Include(r => r.Type)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null) return NotFound();

            return View(result);
        }

        // GET: Admin/Results/Create
        public IActionResult Create()
        {
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Fullname");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectName");
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName");
            return View();
        }

        // POST: Admin/Results/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CandidateId,SubjectId,TypeId,TotalMark,SubmitDate,Status")] Result result)
        {
            if (ModelState.IsValid)
            {
                _context.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Fullname", result.CandidateId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectName", result.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName", result.TypeId);
            return View(result);
        }

        // GET: Admin/Results/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var result = await _context.Results.FindAsync(id);
            if (result == null) return NotFound();

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Fullname", result.CandidateId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectName", result.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName", result.TypeId);
            return View(result);
        }

        // POST: Admin/Results/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CandidateId,SubjectId,TypeId,TotalMark,SubmitDate,Status")] Result result)
        {
            if (id != result.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(result);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Results.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Fullname", result.CandidateId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectName", result.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName", result.TypeId);
            return View(result);
        }

        // GET: Admin/Results/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var result = await _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .Include(r => r.Type)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (result == null) return NotFound();

            return View(result);
        }

        // POST: Admin/Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result != null)
            {
                _context.Results.Remove(result);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
