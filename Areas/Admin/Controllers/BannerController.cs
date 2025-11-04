using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Project_sem_3.Models;
using X.PagedList.Extensions;
namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class BannerController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public BannerController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, int? postion = null, int? active = null)
        {
            int pageSize = 5;
            var LstBanner = _context.Banners.AsQueryable();

            // Lọc theo vị trí (giữ nguyên code của bạn)
            if (postion.HasValue)
            {
                LstBanner = LstBanner.Where(x => x.Postion == postion.Value);
            }

            // 👉 Thêm lọc theo trạng thái
            if (active.HasValue)
            {
                LstBanner = LstBanner.Where(x => x.Active == active.Value);
            }

            // Phân trang
            var LstBanners = LstBanner
                .OrderByDescending(a => a.Id)
                .ToPagedList(page, pageSize);

            // Gửi dữ liệu xuống View
            ViewBag.Postion = postion;
            ViewBag.Status = active;

            return View(LstBanners);
        }



        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Banner obj, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (Image != null && Image.Length > 0)
                {
                    // Tạo thư mục nếu chưa có
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banner");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // Tạo tên file duy nhất
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    // Lưu file vật lý
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }

                    // Lưu đường dẫn tương đối vào DB
                    obj.Image = "/uploads/banner/" + fileName;
                }

                _context.Banners.Add(obj);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Edit(int id)
        {
            
            Banner obj = _context.Banners.FirstOrDefault(c => c.Id == id);
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(Banner obj, IFormFile? NewImage)
        {
            var banner = _context.Banners.FirstOrDefault(b => b.Id == obj.Id);
            if (banner == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin
            banner.Name = obj.Name;
            banner.Description = obj.Description;
            banner.Postion = obj.Postion;
            banner.Active = obj.Active;

            // Nếu có upload ảnh mới
            if (NewImage != null && NewImage.Length > 0)
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(banner.Image))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", banner.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Upload ảnh mới
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banner");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(NewImage.FileName);
                var filePath = Path.Combine(uploadDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    NewImage.CopyTo(stream);
                }

                banner.Image = "/uploads/banner/" + fileName;
            }

            _context.Banners.Update(banner);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var obj = _context.Banners.FirstOrDefault(x => x.Id == id);
            if (obj == null)
                return NotFound();

            // Xóa file ảnh nếu có
            if (!string.IsNullOrEmpty(obj.Image))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", obj.Image.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Banners.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
