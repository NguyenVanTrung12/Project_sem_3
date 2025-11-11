using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    [Route("Admin/Question/Answers")]
    public class AnswersController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public AnswersController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: Admin/Answers
        [AllowAnonymous]
        public IActionResult Index(int page = 1, int? position = null, int? active = null, int? questionId = null)
        {
            int pageSize = 5;

            // Lấy tất cả answers
            var query = _context.Answers.AsQueryable();

            // ✅ Nếu có questionId thì lọc theo câu hỏi
            if (questionId.HasValue)
            {
                query = query.Where(a => a.QuestionId == questionId.Value);
            }

            // ✅ Lọc theo trạng thái
            if (active.HasValue)
            {
                query = query.Where(x => x.Status == active.Value);
            }

            // ✅ Sắp xếp + phân trang
            var pagedAnswers = query
                .OrderByDescending(a => a.Id)
                .ToPagedList(page, pageSize);

            // ✅ Gửi dữ liệu xuống View
            ViewBag.QuestionId = questionId;
            ViewBag.Status = active;

            return View("Index", pagedAnswers);
        }



        // GET: Admin/Answers/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = await _context.Answers
                .Include(a => a.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // GET: Admin/Answers/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create(int questionId)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
                return NotFound();

            // Gán thông tin để hiển thị trong view
            ViewBag.QuestionId = question.Id;
            ViewBag.QuestionTitle = question.QuestionTitle; // 👈 Quan trọng

            return View("Create", new Answer());
        }

        // POST: Admin/Answers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Answer answer, int questionId)
        {
            if (ModelState.IsValid)
            {
                answer.QuestionId = questionId;
                _context.Add(answer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { questionId = answer.QuestionId });
            }

            answer.QuestionId = questionId;
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "QuestionTitle", answer.QuestionId);
            return RedirectToAction("Index", "Questions");
        }

        // GET: Admin/Answers/Edit/5

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var answer = await _context.Answers
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null)
                return NotFound();

            // ✅ Gán SelectList đúng kiểu
            ViewBag.QuestionId = new SelectList(_context.Questions, "Id", "QuestionTitle", answer.QuestionId);

            // Nếu bạn cần quay lại AnswersList của Question đó sau khi cancel
            ViewBag.CurrentQuestionId = answer.QuestionId;

            return View(answer);
        }


        // POST: Admin/Answers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Answer answer)
        {
            if (id != answer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(answer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnswerExists(answer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { questionId = answer.QuestionId });
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "QuestionTitle", answer.QuestionId);
            return View(answer);
        }

        // GET: Admin/Answers/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null) return NotFound();
            return View(answer);
        }

        // POST: Admin/Answers/Delete/5
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            try
            {
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Answers deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                // Kiểm tra nếu lỗi liên quan khóa ngoại
                if (ex.InnerException?.Message.Contains("REFERENCE constraint") == true)
                {
                    TempData["Error"] = "Cannot delete this Answers.";
                }
                else
                {
                    TempData["Error"] = "An unexpected error occurred while deleting the Answers.";
                }
            }
            return RedirectToAction(nameof(Index), new { questionId = answer?.QuestionId });
        }

        private bool AnswerExists(int id)
        {
            return _context.Answers.Any(e => e.Id == id);
        }
    }
}
