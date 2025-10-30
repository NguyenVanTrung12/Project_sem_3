using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Areas.Admin.Helpers;
using Project_sem_3.Models;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class CandidatesController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public CandidatesController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index(string? q, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Candidates
                                           .Include(c => c.Manager)
                                           .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(m => m.Fullname.Contains(q));
            }

            // Sắp xếp + phân trang
            var pagedCandidates = query
                .OrderByDescending(m => m.Id)
                .ToPagedList(page, pageSize);

            // Gửi dữ liệu xuống View
            ViewBag.q = q;

            return View(pagedCandidates);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidate model)
        {
            int managerId = HttpContext.Session.GetInt32("ManagerId") ?? 0;
            if (managerId <= 0)
            {
                TempData["ErrorMessage"] = "Session expired, please log in again.";
                return RedirectToAction("Index", "Logon", new { area = "" });
            }
            model.ManagerId = managerId;

            // ❌ Loại bỏ validate cho navigation property
            ModelState.Remove("Manager");
            ModelState.Remove("Pass");
            var emailExists = await _context.Candidates.AnyAsync(c => c.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already used by another candidate.");
            }
            var phoneExists = await _context.Candidates.AnyAsync(c => c.Phone == model.Phone);
            if (phoneExists)
            {
                ModelState.AddModelError("Phone", "This email is already used by another candidate.");
            }
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    Console.WriteLine($"{state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return View(model);
            }

            try
            {
                var nameParts = model.Fullname?.Trim().Split(' ');
                var lastNamePart = nameParts?.LastOrDefault(); // Lấy tên cuối cùng

                // Loại bỏ dấu và chuyển sang chữ thường
                var lastName = PasswordHelper.RemoveVietnameseAccents(lastNamePart)?.ToLower() ?? "user";

                // Thêm phần thời gian để username không trùng
                var datePart = DateTime.Now.ToString("ddHHmm");
                var username = $"{lastName}{datePart}";

                var plainPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
                var hashedPassword = PasswordHelper.HashPassword(plainPassword);

                // Gán các thông tin tự động
                model.CandidateCode = username.ToUpper();
                model.Pass = hashedPassword;
                model.CreatedAt = DateTime.Now;
                model.Status = 1;

                // Thêm vào DB
                _context.Candidates.Add(model);
                await _context.SaveChangesAsync();

                // Gửi email thông tin tài khoản
                await EmailHelper.SendCandidateAccountEmail(model.Email, username, plainPassword);

                TempData["SuccessMessage"] = $"Candidate added'{model.Fullname}' and send the account via email.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating candidate: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Admin/Candidates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return NotFound();

            return View(candidate);
        }

        // POST: /Admin/Candidates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Candidate model)
        {
            if (id != model.Id)
                return NotFound();

            // Lấy dữ liệu cũ từ DB để giữ username và password
            var candidate = await _context.Candidates.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (candidate == null)
                return NotFound();

            // Giữ nguyên CandidateCode và Pass
            model.CandidateCode = candidate.CandidateCode;
            model.Pass = candidate.Pass;

            // ❌ Loại bỏ validate navigation property nếu có
            ModelState.Remove("Manager");
            var emailExists = await _context.Candidates.AnyAsync(c => c.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already used by another candidate.");
            }

            var phoneExists = await _context.Candidates.AnyAsync(c => c.Phone == model.Phone);
            if (phoneExists)
            {
                ModelState.AddModelError("Phone", "This email is already used by another candidate.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Giữ nguyên ManagerId, CreatedAt nếu cần
                model.ManagerId = candidate.ManagerId;
                model.CreatedAt = candidate.CreatedAt;

                _context.Update(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Candidate information updated '{model.Fullname}'.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateExists(model.Id))
                    return NotFound();
                else
                    throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool CandidateExists(int id)
        {
            return _context.Candidates.Any(e => e.Id == id);
        }
    }

}

