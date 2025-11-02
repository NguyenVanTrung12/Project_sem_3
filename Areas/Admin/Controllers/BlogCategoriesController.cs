using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class BlogCategoriesController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public BlogCategoriesController(online_aptitude_testsContext context)
        {
            _context = context;
        }
      
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string? q, int? status, int page = 1)
        {
            int pageSize = 5;
            var query = _context.BlogCategories
                .AsQueryable();
            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(m => m.CategoryName.Contains(q));
            }
            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            var pagedBlogCate = query
                .OrderByDescending(m => m.Id)
                .ToPagedList(page, pageSize);

            ViewBag.q = q;
            ViewBag.Status = status;

            return View(pagedBlogCate);
        }
        // GET: BlogCategoriesController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: BlogCategoriesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BlogCategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCategory blogCategory)
        {
            if (!ModelState.IsValid)
            {
                // neu du lieu khong hop le quay lai form create va hien thi loi
                return View(blogCategory);
            }
            try
            {
                if (!Request.Form.ContainsKey("Status"))
                {
                    blogCategory.Status = 0;
                }
                _context.BlogCategories.Add(blogCategory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "New addition successful!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while adding the blog category.");
                return View(blogCategory);
            }
        }

        // GET: BlogCategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var blogcate = await _context.BlogCategories.FirstOrDefaultAsync(e => e.Id == id);
            if (blogcate == null)
            {
                TempData["Error"] = "No category found for editing.";
                return RedirectToAction(nameof(Index));
            }

            return View(blogcate);
        }

        // POST: BlogCategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogCategory blogCategory)
        {

            if (!ModelState.IsValid) {
                return View(blogCategory);
            }
            try
            {
                if (!Request.Form.ContainsKey("Status"))
                {
                    blogCategory.Status = 0;
                }
                _context.BlogCategories.Update(blogCategory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Update successfuly";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating:" + ex.Message);
                return View(blogCategory);
            }
        }

        // GET: BlogCategoriesController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var blogCategory = await _context.BlogCategories.FindAsync(id);
            if (blogCategory == null)
            {
                TempData["Error"] = "No blogcategories found to delete!";
                return RedirectToAction(nameof(Index));
            }

            _context.BlogCategories.Remove(blogCategory);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
