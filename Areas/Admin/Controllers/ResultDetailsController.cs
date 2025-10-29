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
    public class ResultDetailsController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public ResultDetailsController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: Admin/ResultDetails
        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;
            var resultDetails = _context.ResultDetails
                .Include(r => r.Result)
                .Include(r => r.Question)
                .Include(r => r.Answer)
                .OrderByDescending(r => r.Id)
                .ToPagedList(page, pageSize);

            return View(resultDetails);
        }

        // GET: Admin/ResultDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var detail = await _context.ResultDetails
                .Include(r => r.Result)
                .Include(r => r.Question)
                .Include(r => r.Answer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (detail == null)
                return NotFound();

            return View(detail);
        }

        // GET: Admin/ResultDetails/Create
        // GET: Admin/ResultDetails/Create
        public IActionResult Create()
        {
            // Chỉ lấy các trường an toàn, tránh EF đọc toàn bộ entity có cột NULL
            ViewData["ResultId"] = new SelectList(
                _context.Results
                    .Where(r => r.Id != null)
                    .Select(r => new { r.Id })
                    .ToList(),
                "Id", "Id"
            );

            ViewData["QuestionId"] = new SelectList(
                _context.Questions
                    .Where(q => q.QuestionTitle != null)
                    .Select(q => new { q.Id, q.QuestionTitle })
                    .ToList(),
                "Id", "QuestionTitle"
            );

            ViewData["AnswerId"] = new SelectList(
                _context.Answers
                    .Where(a => a.AnswerContent != null)
                    .Select(a => new { a.Id, a.AnswerContent })
                    .ToList(),
                "Id", "AnswerContent"
            );

            return View();
        }



        // POST: Admin/ResultDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ResultId,QuestionId,AnswerId,Mark")] ResultDetail detail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ResultId"] = new SelectList(_context.Results, "Id", "Id", detail.ResultId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "QuestionTitle", detail.QuestionId);
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "AnswerContent", detail.AnswerId);
            return View(detail);
        }

        // GET: Admin/ResultDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var detail = await _context.ResultDetails.FindAsync(id);
            if (detail == null)
                return NotFound();

            ViewData["ResultId"] = new SelectList(
                _context.Results.Where(r => r.Id != null).ToList(),
                "Id", "Id", detail.ResultId
            );
            ViewData["QuestionId"] = new SelectList(
                _context.Questions.Where(q => q.Id != null && q.QuestionTitle != null).ToList(),
                "Id", "QuestionTitle", detail.QuestionId
            );
            ViewData["AnswerId"] = new SelectList(
                _context.Answers.Where(a => a.Id != null && a.AnswerContent != null).ToList(),
                "Id", "AnswerContent", detail.AnswerId
            );

            return View(detail);
        }


        // POST: Admin/ResultDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ResultId,QuestionId,AnswerId,Mark")] ResultDetail detail)
        {
            if (id != detail.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ResultDetails.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ResultId"] = new SelectList(_context.Results, "Id", "Id", detail.ResultId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "QuestionTitle", detail.QuestionId);
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "AnswerContent", detail.AnswerId);
            return View(detail);
        }

        // GET: Admin/ResultDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var detail = await _context.ResultDetails
                .Include(r => r.Result)
                .Include(r => r.Question)
                .Include(r => r.Answer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (detail == null)
                return NotFound();

            return View(detail);
        }

        // POST: Admin/ResultDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detail = await _context.ResultDetails.FindAsync(id);
            if (detail != null)
            {
                _context.ResultDetails.Remove(detail);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
