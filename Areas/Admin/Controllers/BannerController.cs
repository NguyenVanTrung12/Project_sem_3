using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Project_sem_3.Models;
using X.PagedList.Extensions;
namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        
        private  online_aptitude_testsContext tr = new online_aptitude_testsContext();

        public IActionResult Index(int page = 1, int? postion = null)
        {
            int pageSize = 5;
            var LstBanner = tr.Banners.AsQueryable();

            if (postion.HasValue)
            {
                LstBanner = LstBanner.Where(x => x.Postion == postion.Value);
            }

            // Phân trang
            var LstBanners = LstBanner
                .OrderByDescending(a => a.Id)
                .ToPagedList(page, pageSize);

            // Gửi dữ liệu xuống View
            ViewBag.Postion = postion;
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

                tr.Banners.Add(obj);
                tr.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Edit(int id)
        {
            
            Banner obj = tr.Banners.FirstOrDefault(c => c.Id == id);
            return View(obj);
        }
        [HttpPost]
        [HttpPost]
        public IActionResult Edit(Banner obj, IFormFile? NewImage)
        {
            var banner = tr.Banners.FirstOrDefault(b => b.Id == obj.Id);
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

            tr.Banners.Update(banner);
            tr.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Banner obj = tr.Banners.FirstOrDefault(x => x.Id == id);
            tr.Banners.Remove(obj);
            tr.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
