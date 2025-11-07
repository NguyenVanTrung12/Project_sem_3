using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using X.PagedList;
using X.PagedList.Extensions;


namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class SubjectsController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public SubjectsController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public IActionResult Index(string? searchString, int? status, int? page)
        {
            var query = _context.Subjects.AsQueryable();

            // Tìm kiếm theo tên môn học hoặc mã môn học
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => (s.SubjectName != null && s.SubjectName.Contains(searchString))
                                    || (s.SubjectCode != null && s.SubjectCode.Contains(searchString)));
                ViewBag.CurrentSearch = searchString;
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
                ViewBag.CurrentStatus = status.Value;
            }

            // Sắp xếp mặc định theo Id
            query = query.OrderBy(s => s.Id);

            // Phân trang
            int pageSize = 5; // Số item trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là 1

            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = pageNumber;

            return View(query.ToPagedList(pageNumber, pageSize));
        }


        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectCode,SubjectName,LimitTime,QuestionCount,OrderIndex,Status")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectCode,SubjectName,LimitTime,QuestionCount,OrderIndex,Status")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
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
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Questions)
                .Include(s => s.Results)
                .Include(s => s.SubjectTypes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                TempData["Error"] = "❌ Subject not found!";
                return RedirectToAction(nameof(Index));
            }

            if (subject.Questions.Any())
            {
                TempData["Error"] = "⚠️ Cannot delete this subject because it already has linked questions.";
                return RedirectToAction(nameof(Index));
            }

            if (subject.Results.Any())
            {
                TempData["Error"] = "⚠️ Cannot delete this subject because it already has linked results.";
                return RedirectToAction(nameof(Index));
            }

            if (subject.SubjectTypes.Any())
            {
                TempData["Error"] = "⚠️ Cannot delete this subject because it already has linked subject types.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Subject deleted successfully!";
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("REFERENCE"))
                {
                    TempData["Error"] = "⚠️ Cannot delete this subject because it is referenced by other data.";
                }
                else
                {
                    TempData["Error"] = $"❌ Unexpected error: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(Index));
        }
        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
