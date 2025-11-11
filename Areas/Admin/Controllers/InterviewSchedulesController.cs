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
            PopulateInterviewerDropdown(); // ← thêm dòng này
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
            PopulateInterviewerDropdown(interview.Interviewer);
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
            PopulateInterviewerDropdown(interview.Interviewer);
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
                    TempData["Success"] = "Updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error while editing" + ex.Message);
                }
            }
            PopulateInterviewerDropdown(interviewSchedule.Interviewer);
            return View(interviewSchedule);
        }

        // GET: BlogsController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var interview = await _context.InterviewSchedules.FindAsync(id);
            if (interview == null)
            {
                TempData["Error"] = "No Blogs found to delete!";
                return RedirectToAction(nameof(Index));
            }

            _context.InterviewSchedules.Remove(interview);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Blog deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        private void PopulateInterviewerDropdown(string? selectedInterviewer = null)
        {
            var hrList = (from m in _context.Managers
                          join r in _context.Roles on m.RoleId equals r.Id
                          where r.RoleName == "Role_HR"
                          orderby m.Fullname
                          select m).ToList();

            if (hrList == null)
                hrList = new List<Manager>();

            // Loại bỏ item FullName null
            hrList = hrList.Where(x => !string.IsNullOrEmpty(x.Fullname)).ToList();

            ViewBag.InterviewerList = new SelectList(hrList, "Fullname", "Fullname", selectedInterviewer);
        }



    }
}
