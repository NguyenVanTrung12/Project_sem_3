using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Areas.Admin.Helpers;
using Project_sem_3.Models;
using System.Reflection.Metadata;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers")]
    public class ManagerController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public ManagerController(online_aptitude_testsContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult Index(string? q, int? status, string? role, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Managers
                                .Include(m => m.Role)
                                .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(q))
                query = query.Where(m => m.Fullname.Contains(q));

            // Lọc theo trạng thái
            if (status.HasValue)
                query = query.Where(m => m.Status == status.Value);

            // Lọc theo role
            if (!string.IsNullOrEmpty(role))
                query = query.Where(m => m.Role.RoleName == role);

            // Sắp xếp: luôn để role_supper_manager lên đầu
            var pagedManagers = query
                .OrderByDescending(m => m.Role.RoleName == "Role_Supper_Managers")
                .ThenByDescending(m => m.Id)
                .ToPagedList(page, pageSize);

            // Lấy danh sách role từ DB để hiển thị select
            ViewBag.Roles = _context.Roles.ToList();
            ViewBag.q = q;
            ViewBag.Status = status;
            ViewBag.SelectedRole = role;

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
                // 1️⃣ Check trùng username
                var usernameExists = await _context.Managers
                    .AnyAsync(m => m.Username == model.Username);
                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", model.RoleId);
                    return View(model);
                }

                // 2️⃣ Check trùng email
                var emailExists = await _context.Managers
                    .AnyAsync(m => m.Email == model.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", model.RoleId);
                    return View(model);
                }

                var phoneExists = await _context.Managers
                  .AnyAsync(m => m.Phone == model.Phone);
                if (emailExists)
                {
                    ModelState.AddModelError("Phone", "Phone already exists.");
                    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", model.RoleId);
                    return View(model);
                }

                // 3️⃣ Hash password
                model.PasswordHash = PasswordHelper.HashPassword(model.PasswordHash);

                // 4️⃣ Status mặc định nếu không chọn
                if (!Request.Form.ContainsKey("Status"))
                {
                    model.Status = 0;
                }

                // 5️⃣ Thêm vào DB
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
                return NotFound();

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

                // 1️⃣ Kiểm tra trùng Username (loại trừ bản ghi hiện tại)
                var usernameExists = await _context.Managers
                    .AnyAsync(m => m.Username == model.Username && m.Id != id);
                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", model.RoleId);
                    return PartialView("Edit", model);
                }

                // 2️⃣ Kiểm tra trùng Email (loại trừ bản ghi hiện tại)
                var emailExists = await _context.Managers
                    .AnyAsync(m => m.Email == model.Email && m.Id != id);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", model.RoleId);
                    return PartialView("Edit", model);
                }

                var phoneExists = await _context.Managers
                 .AnyAsync(m => m.Phone == model.Phone && m.Id != id);
                if (emailExists)
                {
                    ModelState.AddModelError("Phone", "Phone already exists.");
                    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "RoleName", model.RoleId);
                    return PartialView("Edit", model);
                }

                // 3️⃣ Cập nhật các trường
                manager.Username = model.Username;
                manager.Fullname = model.Fullname;
                manager.Email = model.Email;
                manager.Phone = model.Phone;
                manager.RoleId = model.RoleId;
                manager.Status = model.Status ?? 0;
                manager.CreatedAt = model.CreatedAt;

                // 4️⃣ Nếu nhập mật khẩu mới
                if (!string.IsNullOrWhiteSpace(model.PasswordHash))
                {
                    manager.PasswordHash = PasswordHelper.HashPassword(model.PasswordHash);
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


