using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogsController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public BlogsController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: BlogsController
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;

            var blogs = _context.Blogs
                .Include(e => e.BlogCategory)
                .Include(e => e.Manager)
                .OrderByDescending(e => e.Id)
                .ToPagedList(page, pageSize);

            return View(blogs);
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
                if (uploadfile is { Length: > 0 })
                {
                    var ext = Path.GetExtension(uploadfile.FileName).ToLower();
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
                    if (!allowedExts.Contains(ext))
                    {
                        ModelState.AddModelError("", "Chỉ được upload ảnh JPG/PNG");
                        return View(blog);
                    }

                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                    Directory.CreateDirectory(uploads);

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploads, fileName);

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadfile.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn tương đối vào DB
                    blog.Image = $"/uploads/images/{fileName}";
                }
                try
                {
                    _context.Blogs.Add(blog);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Thêm thành công";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm" + ex.Message);
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


                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thành công";
                    return RedirectToAction(nameof(Index));
                }
                
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi sửa" + ex.Message);
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
                TempData["Error"] = "Không tìm thấy Blogs để xoá!";
                return RedirectToAction(nameof(Index));
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xoá blog thành công!";
            return RedirectToAction(nameof(Index));
        }

    }
}
