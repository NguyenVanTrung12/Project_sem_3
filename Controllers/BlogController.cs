using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Threading.Tasks;

namespace Project_sem_3.Controllers
{
    public class BlogController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public BlogController(online_aptitude_testsContext context)
        {
            _context = context;
        }


        [Route("/blog")]
        public async Task<IActionResult> Index(string? q, int? categoryId)
        {
            var categories = await _context.BlogCategories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.SearchQuery = q;
            ViewBag.SelectedCategory = categoryId;

            IQueryable<Blog> query = _context.Blogs.Include(b => b.BlogCategory);

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(b => b.Title.Contains(q) || b.Content.Contains(q));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.BlogCategoryId == categoryId.Value);
            }

            var blogs = await query
                .OrderByDescending(b => b.CreatedAt)
                .Take(9)
                .ToListAsync();

            return View(blogs);
        }

        [HttpGet]
        [Route("/blog-detail/{Slug}")]
        public async Task<IActionResult> BlogDetails(string slug)
        {
            var blog = await _context.Blogs
           .Include(b => b.BlogCategory)
           .FirstOrDefaultAsync(b => b.Slug == slug);

            if (blog == null)
            {
                return NotFound();
            }

            ViewBag.RecentBlogs = await _context.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.Categories = await _context.BlogCategories.ToListAsync();

            return View(blog);
        }
    }
}
