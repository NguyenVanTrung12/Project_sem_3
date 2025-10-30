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
using Microsoft.AspNetCore.Authorization;


namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class TypesController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public TypesController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: Types
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, int? page, int? status)
        {

            var query = _context.Types
                .Include(s => s.SubjectTypes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(t => t.TypeName != null && t.TypeName.Contains(searchString));
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

            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Use X.PagedList to page the queryable directly
            var paged = query.OrderBy(st => st.Id).ToPagedList(pageNumber, pageSize);
            return View(paged);
        }

        // GET: Types/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @type = await _context.Types
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@type == null)
            {
                return NotFound();
            }

            return View(@type);
        }

        // GET: Types/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Types/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypeName,Status,Ord")] TypeModel @type)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@type);
        }

        // GET: Types/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @type = await _context.Types.FindAsync(id);
            if (@type == null)
            {
                return NotFound();
            }
            return View(@type);
        }

        // POST: Types/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeName,Status,Ord")] TypeModel @type)
        {
            if (id != @type.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@type);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeExists(@type.Id))
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
            return View(@type);
        }

        // GET: Types/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @type = await _context.Types
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@type == null)
            {
                return NotFound();
            }

            return View(@type);
        }

        // POST: Types/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var type = await _context.Types.FindAsync(id);
            if (type == null)
            {
                return NotFound();
            }

            // Kiểm tra xem subject có đang được sử dụng ở bảng khác không
            bool hasRelations = await _context.SubjectTypes.AnyAsync(st => st.TypeId == id);
                                

            if (hasRelations)
            {
                TempData["ErrorMessage"] = "❌ Cannot delete this type because it is linked to other data (SubjectType, Question, or Result).";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Types.Remove(type);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Successfully deleted type!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "⚠️ An error occurred while deleting the type.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TypeExists(int id)
        {
            return _context.Types.Any(e => e.Id == id);
        }
    }
}
