using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using TypeModel = Project_sem_3.Models.Type;
using X.PagedList;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers

{
    [Area("Admin")] 
    public class SubjectTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

    // GET: SubjectTypes
    public IActionResult Index(string searchString, int? page)
        {
            var query = _context.SubjectType
                .Include(s => s.Subject)
                .Include(s => s.Type)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(st => (st.Subject != null && st.Subject.SubjectName != null && st.Subject.SubjectName.Contains(searchString))
                                         || (st.Type != null && st.Type.TypeName != null && st.Type.TypeName.Contains(searchString)));
                ViewBag.CurrentSearch = searchString;
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Use X.PagedList to page the queryable directly
            var paged = query.OrderBy(st => st.Id).ToPagedList(pageNumber, pageSize);
            return View(paged);
        }

        // GET: SubjectTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectType = await _context.SubjectType
                .Include(s => s.Subject)
                .Include(s => s.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectType == null)
            {
                return NotFound();
            }

            return View(subjectType);
        }

        // GET: SubjectTypes/Create
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(
                _context.Subject.Select(s => new { s.Id, DisplayName = s.Id + " - " + s.SubjectName }),
                "Id",
                "DisplayName"
            );
            ViewData["TypeId"] = new SelectList(
                _context.Type.Select(t => new { t.Id, DisplayName = t.Id + " - " + t.TypeName }),
                "Id",
                "DisplayName"
            );
            return View();
        }

        // POST: SubjectTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectId,TypeId,NumQuestion,LimitTime")] SubjectType subjectType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subjectType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Id", subjectType.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Set<TypeModel>(), "Id", "Id", subjectType.TypeId);
            return View(subjectType);
        }

        // GET: SubjectTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectType = await _context.SubjectType.FindAsync(id);
            if (subjectType == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(
                _context.Subject.Select(s => new { s.Id, DisplayName = s.Id + " - " + s.SubjectName }),
                "Id",
                "DisplayName"
            );
            ViewData["TypeId"] = new SelectList(
                _context.Type.Select(t => new { t.Id, DisplayName = t.Id + " - " + t.TypeName }),
                "Id",
                "DisplayName"
            );
            return View(subjectType);
        }

        // POST: SubjectTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectId,TypeId,NumQuestion,LimitTime")] SubjectType subjectType)
        {
            if (id != subjectType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subjectType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectTypeExists(subjectType.Id))
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
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Id", subjectType.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Set<TypeModel>(), "Id", "Id", subjectType.TypeId);
            return View(subjectType);
        }

        // GET: SubjectTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectType = await _context.SubjectType
                .Include(s => s.Subject)
                .Include(s => s.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectType == null)
            {
                return NotFound();
            }

            return View(subjectType);
        }

        // POST: SubjectTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subjectType = await _context.SubjectType.FindAsync(id);
            if (subjectType != null)
            {
                _context.SubjectType.Remove(subjectType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectTypeExists(int id)
        {
            return _context.SubjectType.Any(e => e.Id == id);
        }
    }
}
