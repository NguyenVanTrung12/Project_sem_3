using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Threading.Tasks;
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class BlogsController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public BlogsController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: BlogsController
        [HttpGet]
        public IActionResult Index(string? q, int? status,int page = 1)
        {
            int pageSize = 5;
            var query = _context.Blogs
                .Include(e => e.BlogCategory)
                .Include(e => e.Manager)
                .AsQueryable();
            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(m => m.Title.Contains(q));
            }
            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            var pagedBlog = query
                .OrderByDescending(m => m.Id)
                .ToPagedList(page, pageSize);

            ViewBag.q = q;
            ViewBag.Status = status;

            return View(pagedBlog);
        }


        // GET: BlogsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BlogsController/Create
        public ActionResult Create()
        {
            ViewBag.BlogCategoriList = new SelectList(_context.BlogCategories, "Id", "CategoryName");
            ViewBag.ManagerList = new SelectList(_context.Managers, "Id", "Fullname");
            return View();
        }

        // POST: BlogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Blog blog, IFormFile? uploadfile)
        {
            ViewBag.BlogCategoriList = new SelectList(_context.BlogCategories, "Id", "CategoryName");
            ViewBag.ManagerList = new SelectList(_context.Managers, "Id", "Fullname");
            if (ModelState.IsValid)
            {
                //xử lý upload ảnh
                if (uploadfile?.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                    Directory.CreateDirectory(uploads);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadfile.FileName)}";
                    var filePath = Path.Combine(uploads, fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await uploadfile.CopyToAsync(stream);

                    blog.Image = $"/uploads/images/{fileName}";
                }
                try
                {
                    if (!Request.Form.ContainsKey("Status"))
                    {
                        blog.Status = 0;
                    }
                    _context.Blogs.Add(blog);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "More success";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error while adding" + ex.Message);
                }

            }
            return View(blog);
        }


        // GET: BlogsController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.BlogCategoriList = new SelectList(_context.BlogCategories, "Id", "CategoryName");
            ViewBag.ManagerList = new SelectList(_context.Managers, "Id", "Fullname");
            var blog = await _context.Blogs.FirstOrDefaultAsync(e => e.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: BlogsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Blog blog,IFormFile? uploadfile)
        {
            ViewBag.BlogCategoriList = new SelectList(_context.BlogCategories, "Id", "CategoryName");
            ViewBag.ManagerList = new SelectList(_context.Managers, "Id", "Fullname");
            if (id != blog.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {

                    var existingBlog = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                    if (existingBlog == null) return NotFound();

                    // Xử lý upload ảnh
                    if (uploadfile?.Length > 0)
                    {
                        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                        Directory.CreateDirectory(uploads);

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadfile.FileName)}";
                        var filePath = Path.Combine(uploads, fileName);

                        await using var stream = new FileStream(filePath, FileMode.Create);
                        await uploadfile.CopyToAsync(stream);

                        // Xóa file cũ nếu có
                        if (!string.IsNullOrEmpty(existingBlog.Image))
                        {
                            var oldFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingBlog.Image.TrimStart('/'));
                            if (System.IO.File.Exists(oldFile))
                                System.IO.File.Delete(oldFile);
                        }

                        blog.Image = $"/uploads/images/{fileName}";
                    }
                    else
                    {
                        blog.Image = existingBlog.Image;
                    }
                    if (!Request.Form.ContainsKey("Status"))
                    {
                        blog.Status = 0;
                    }

                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error while editing" + ex.Message);
                }
            }
            return View(blog);
        }

        // GET: BlogsController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                TempData["Error"] = "No Blog found to delete!";
                return RedirectToAction(nameof(Index));
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Blog deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
