using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;

namespace Project_sem_3.Areas.Admin.Controllers.Controllers
{
    public class ResultDetailsController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public ResultDetailsController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: ResultDetails
        public async Task<IActionResult> Index()
        {
            var online_aptitude_testsContext = _context.ResultDetails.Include(r => r.Answer).Include(r => r.Question).Include(r => r.Result);
            return View(await online_aptitude_testsContext.ToListAsync());
        }

        // GET: ResultDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resultDetail = await _context.ResultDetails
                .Include(r => r.Answer)
                .Include(r => r.Question)
                .Include(r => r.Result)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resultDetail == null)
            {
                return NotFound();
            }

            return View(resultDetail);
        }

        // GET: ResultDetails/Create
        public IActionResult Create()
        {
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "Id");
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "QuestionContent");
            ViewData["ResultId"] = new SelectList(_context.Results, "Id", "Id");
            return View();
        }

        // POST: ResultDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ResultId,QuestionId,AnswerId,Mark")] ResultDetail resultDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resultDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "Id", resultDetail.AnswerId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "QuestionContent", resultDetail.QuestionId);
            ViewData["ResultId"] = new SelectList(_context.Results, "Id", "Id", resultDetail.ResultId);
            return View(resultDetail);
        }

        // GET: ResultDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resultDetail = await _context.ResultDetails.FindAsync(id);
            if (resultDetail == null)
            {
                return NotFound();
            }
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "Id", resultDetail.AnswerId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "QuestionContent", resultDetail.QuestionId);
            ViewData["ResultId"] = new SelectList(_context.Results, "Id", "Id", resultDetail.ResultId);
            return View(resultDetail);
        }

        // POST: ResultDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ResultId,QuestionId,AnswerId,Mark")] ResultDetail resultDetail)
        {
            if (id != resultDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resultDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResultDetailExists(resultDetail.Id))
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
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "Id", resultDetail.AnswerId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "QuestionContent", resultDetail.QuestionId);
            ViewData["ResultId"] = new SelectList(_context.Results, "Id", "Id", resultDetail.ResultId);
            return View(resultDetail);
        }

        // GET: ResultDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resultDetail = await _context.ResultDetails
                .Include(r => r.Answer)
                .Include(r => r.Question)
                .Include(r => r.Result)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resultDetail == null)
            {
                return NotFound();
            }

            return View(resultDetail);
        }

        // POST: ResultDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resultDetail = await _context.ResultDetails.FindAsync(id);
            if (resultDetail != null)
            {
                _context.ResultDetails.Remove(resultDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResultDetailExists(int id)
        {
            return _context.ResultDetails.Any(e => e.Id == id);
        }
    }
}
