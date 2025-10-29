using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Areas.Admin.Helpers;
using Project_sem_3.Models;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ManagerController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public ManagerController(online_aptitude_testsContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult Index(string? q, int? status, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Managers
                                .Include(m => m.Role)
                                .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(m => m.Fullname.Contains(q));
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            // Sắp xếp + phân trang
            var pagedManagers = query
                .OrderByDescending(m => m.Id)
                .ToPagedList(page, pageSize);

            // Gửi dữ liệu xuống View
            ViewBag.q = q;
            ViewBag.Status = status;

            return View(pagedManagers);
        }
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName");
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Manager model)
        {
            if (ModelState.IsValid)
            {
                model.PasswordHash = PasswordHelper.HashPassword(model.PasswordHash);

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", model.RoleId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
                return NotFound();

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", manager.RoleId);
            return View(manager);
        }

        // Cập nhật (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Manager model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", model.RoleId);
                return PartialView("Edit", model);
            }

            try
            {
                // Lấy bản ghi hiện tại trong DB
                var manager = await _context.Managers.FindAsync(id);
                if (manager == null)
                    return NotFound();

                // Cập nhật các trường khác
                manager.Username = model.Username;
                manager.Fullname = model.Fullname;
                manager.Email = model.Email;
                manager.Phone = model.Phone;
                manager.RoleId = model.RoleId;
                manager.Status = model.Status;
                manager.CreatedAt = model.CreatedAt;

                // Nếu có nhập mật khẩu mới
                if (!string.IsNullOrWhiteSpace(manager.PasswordHash))
                {
                    manager.PasswordHash = PasswordHelper.HashPassword(manager.PasswordHash);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagerExists(model.Id))
                    return NotFound();
                throw;
            }
        }
        // Form xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var manager = await _context.Managers
                                        .Include(m => m.Role)
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null)
                return NotFound();

            return View(manager);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ManagerExists(int id)
        {
            return _context.Managers.Any(e => e.Id == id);
        }
    }
}


