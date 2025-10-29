using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    public class ResultsController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public ResultsController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: Results

        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var results = _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .Include(r => r.Type)
                .OrderByDescending(r => r.Id)
                .ToPagedList(pageNumber, pageSize);

            return View(results);
        }



        // GET: Results/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .Include(r => r.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: Results/Create
        public IActionResult Create()
        {
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Address");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id");
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Id");
            return View();
        }

        // POST: Results/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Address", result.CandidateId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", result.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Id", result.TypeId);
            return View(result);
        }

        // GET: Results/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Address", result.CandidateId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", result.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Id", result.TypeId);
            return View(result);
        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CandidateId,SubjectId,TypeId,TotalMark,SubmitDate,Status")] Result result)
        {
            if (id != result.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(result);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResultExists(result.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Address", result.CandidateId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", result.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Id", result.TypeId);
            return View(result);
        }

        // GET: Results/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .Include(r => r.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result != null)
            {
                _context.Results.Remove(result);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResultExists(int id)
        {
            return _context.Results.Any(e => e.Id == id);
        }
    }
}
