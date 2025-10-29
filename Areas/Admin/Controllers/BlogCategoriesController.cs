using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogCategoriesController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public BlogCategoriesController(online_aptitude_testsContext context)
        {
            _context = context;
        }
        // GET: BlogCategoriesController
        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;
            var blogcate =  _context.BlogCategories.OrderByDescending(e => e.Id).ToPagedList(page,pageSize);
            return View(blogcate);
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
                _context.BlogCategories.Add(blogCategory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm  mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm danh mục blog.");
                return View(blogCategory);
            }
        }

        // GET: BlogCategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var blogcate = await _context.BlogCategories.FirstOrDefaultAsync(e => e.Id == id);
            if (blogcate == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục cần sửa.";
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
                _context.BlogCategories.Update(blogCategory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Update successfuly";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật:" + ex.Message);
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
                TempData["Error"] = "Không tìm thấy blogcategories để xoá!";
                return RedirectToAction(nameof(Index));
            }

            _context.BlogCategories.Remove(blogCategory);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xoá danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }

    }
}
