using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Areas.Admin.Helpers;
using Project_sem_3.Models;
using System.Reflection.Metadata;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public QuestionController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: /Admin/Question
        public IActionResult Index(string? q, int? status, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Questions
                .Include(q => q.Subject) // nếu có quan hệ
                .Include(q => q.Type)    // nếu có quan hệ
                
                                .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(m => m.QuestionTitle.Contains(q));
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            // Sắp xếp + phân trang
            var pagedQuestions = query
                .OrderByDescending(m => m.Id)
                .ToPagedList(page, pageSize);

            // Gửi dữ liệu xuống View
            ViewBag.q = q;
            ViewBag.Status = status;

            return View(pagedQuestions);
        }
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName");
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "TypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question model)
        {
            if (ModelState.IsValid)
            {
                if (!Request.Form.ContainsKey("Status"))
                {
                    model.Status = 0;
                }
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Nếu validation lỗi, load lại dropdown
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectName", model.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName", model.TypeId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
                return NotFound();

            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectName", question.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName", question.TypeId);
            return View(question);
        }

        // POST: /Admin/Question/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Question model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (!Request.Form.ContainsKey("Status"))
                    {
                        model.Status = 0;
                    }
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(model.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectName", model.SubjectId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "TypeName", model.TypeId);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}