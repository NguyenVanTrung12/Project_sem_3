using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class InterviewSchedulesController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public InterviewSchedulesController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: BlogsController
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;
            var interview =  _context.InterviewSchedules
                .Include(e => e.Candidate)
                .OrderByDescending(e => e.Id)
                .ToPagedList(page,pageSize);
            return View(interview);
        }

        // GET: BlogsController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: BlogsController/Create
        public IActionResult Create()
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");

            return View();
        }

        // POST: BlogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InterviewSchedule interview)
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "FullName");

            if (ModelState.IsValid)
            {
                // Nếu người dùng không chọn ngày => tự gán ngày hiện tại
                if (!interview.InterviewDate.HasValue)
                {
                    interview.InterviewDate = DateTime.Now;
                }

                _context.Add(interview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(interview);
        }



        // GET: BlogsController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            var interview = await _context.InterviewSchedules.FirstOrDefaultAsync(e => e.Id == id);
            if (interview == null)
            {
                return NotFound();
            }
            return View(interview);
        }

        // POST: BlogsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InterviewSchedule interviewSchedule)
        {

            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");

            if (id != interviewSchedule.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(interviewSchedule);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thành công";
                    return RedirectToAction(nameof(Index));
                }

                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi sửa" + ex.Message);
                }
            }
            return View(interviewSchedule);
        }

        // GET: BlogsController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var interview = await _context.InterviewSchedules.FindAsync(id);
            if (interview == null)
            {
                TempData["Error"] = "Không tìm thấy Blogs để xoá!";
                return RedirectToAction(nameof(Index));
            }

            _context.InterviewSchedules.Remove(interview);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xoá blog thành công!";
            return RedirectToAction(nameof(Index));
        }

    }
}
