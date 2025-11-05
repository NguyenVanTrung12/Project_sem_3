using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Reflection.Metadata;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class ResultsController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public ResultsController(online_aptitude_testsContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? q, int? status, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Results.Include(e => e.Candidate).Include(e => e.Subject).Include(e => e.Type).AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(m => m.Candidate.Fullname.Contains(q));
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            // Sắp xếp + phân trang
            var pagedResult = query
                .OrderByDescending(m => m.Id)
                .ToPagedList(page, pageSize);

            // Gửi dữ liệu xuống View
            ViewBag.q = q;
            ViewBag.Status = status;

            return View(pagedResult);
        }


        public IActionResult Details(int id)
        {
            // Lấy result + các liên kết cần thiết
            var result = _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .Include(r => r.Type)
                .Include(r => r.ResultDetails)
                    .ThenInclude(rd => rd.Question)
                .Include(r => r.ResultDetails)
                    .ThenInclude(rd => rd.Answer)
                .FirstOrDefault(r => r.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }


        public IActionResult Create()
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            ViewBag.SubjectList = new SelectList(_context.Subjects, "Id", "SubjectName");
            ViewBag.TypeList = new SelectList(_context.Types, "Id", "TypeName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Result result)
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            ViewBag.SubjectList = new SelectList(_context.Subjects, "Id", "SubjectName");
            ViewBag.TypeList = new SelectList(_context.Types, "Id", "TypeName");
            if (!ModelState.IsValid)
            {
                return View(result);
            }
            try
            {
                if (!Request.Form.ContainsKey("Status"))
                {
                    result.Status = 0;
                }
                _context.Results.Add(result);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Add successfuly";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while adding the transfer.");
                return View(result);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            ViewBag.SubjectList = new SelectList(_context.Subjects, "Id", "SubjectName");
            ViewBag.TypeList = new SelectList(_context.Types, "Id", "TypeName");
            var result = await _context.Results.FirstOrDefaultAsync(e => e.Id == id);
            if (result == null)
            {
                TempData["Error"] = "No result found to edit.";
                return RedirectToAction(nameof(Index));
            }

            return View(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Result result)
        {
            ViewBag.CandidateList = new SelectList(_context.Candidates, "Id", "Fullname");
            ViewBag.SubjectList = new SelectList(_context.Subjects, "Id", "SubjectName");
            ViewBag.TypeList = new SelectList(_context.Types, "Id", "TypeName");
            if (!ModelState.IsValid)
            {
                return View(result);
            }
            try
            {
                if (!Request.Form.ContainsKey("Status"))
                {
                    result.Status = 0;
                }
                _context.Results.Update(result);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Edit successfuly";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating:" + ex.Message);
                return View(result);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                TempData["Error"] = "No transfe found to delete!";
                return RedirectToAction(nameof(Index));
            }

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Delete successfuly";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                TempData["Error"] = "Result not found.";
                return NotFound();
            }

            bool hasRelations = await _context.ResultDetails.AnyAsync(r => r.ResultId == id);
            if (hasRelations)
            {
                TempData["Error"] = "Cannot delete this Result because it is associated with existing Result Details.";
                return RedirectToAction("Index");
            }
            try
            {
                _context.Results.Remove(result);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Result deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while deleting the Result: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
