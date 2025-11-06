using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Diagnostics;

namespace Project_sem_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly online_aptitude_testsContext _context;

        public HomeController(ILogger<HomeController> logger, online_aptitude_testsContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ✅ Trang chủ hiển thị danh sách phần thi
        public async Task<IActionResult> Index()
        {
            var sessionCandidateId = HttpContext.Session.GetInt32("CandidateId");
            int? candidateId = sessionCandidateId;

            var subjects = await _context.Subjects.OrderBy(s => s.Id).ToListAsync();

            var results = candidateId != null
                ? await _context.Results
                    .Where(r => r.CandidateId == candidateId.Value)
                    .ToListAsync()
                : new List<Result>();

            bool blocked = false; // Nếu trượt ở vòng nào → khóa tất cả vòng sau
            bool lockedDueToFail = false;

            foreach (var subject in subjects)
            {
                var currentResult = results.FirstOrDefault(r => r.SubjectId == subject.Id);

                bool isDone = currentResult?.Status == 1;   // 1 = Đậu
                bool isFailed = currentResult?.Status == 2; // 2 = Trượt
                bool canAccess = false;

                // Nếu đã bị khóa trước đó => khóa tiếp
                if (blocked)
                {
                    canAccess = false;
                }
                else
                {
                    if (subject.Id == subjects.First().Id)
                    {
                        // Vòng đầu mở nếu chưa trượt
                        canAccess = !isFailed;
                    }
                    else
                    {
                        // Lấy vòng trước
                        var prevSubject = subjects
                            .OrderBy(s => s.Id)
                            .TakeWhile(s => s.Id != subject.Id)
                            .LastOrDefault();

                        var prevResult = prevSubject != null
                            ? results.FirstOrDefault(r => r.SubjectId == prevSubject.Id)
                            : null;

                        // Mở nếu vòng trước đậu
                        canAccess = (prevResult != null && prevResult.Status == 1) && !isFailed;
                    }
                }

                // Nếu vòng này trượt → khóa tất cả vòng sau
                if (isFailed)
                {
                    blocked = true;
                    lockedDueToFail = true;
                    canAccess = false;
                }

                ViewData[$"CanAccess_{subject.Id}"] = canAccess;
                ViewData[$"IsDone_{subject.Id}"] = isDone;
                ViewData[$"IsFailed_{subject.Id}"] = isFailed;
                ViewData[$"LockedDueToFail_{subject.Id}"] = lockedDueToFail;
            }

            ViewData["IsLoggedIn"] = candidateId != null;

            return View(subjects);
        }






        // ✅ Hàm logic kiểm tra điều kiện mở phần thi
        private bool CanAccess(int subjectId, List<Result> results)
        {
            // ⚠️ Phần 1 (Kiến thức chung)
            if (subjectId == 1)
            {
                var firstResult = results.FirstOrDefault(r => r.SubjectId == 1);
                // Chỉ cho phép nếu chưa thi hoặc chưa trượt
                return firstResult == null || firstResult.Status != 2;
            }

            // ✅ Các phần còn lại: chỉ mở nếu phần trước đậu
            for (int prevId = 1; prevId < subjectId; prevId++)
            {
                var prevResult = results.FirstOrDefault(r => r.SubjectId == prevId);

                // Nếu chưa có kết quả phần trước → khóa
                if (prevResult == null)
                    return false;

                // Nếu phần trước bị trượt → khóa luôn
                if (prevResult.Status == 2)
                    return false;

                // Nếu phần trước chưa đậu → khóa
                if (prevResult.Status != 1)
                    return false;
            }

            // Nếu tất cả phần trước đều đã đậu → mở khóa phần này
            return true;
        }





    }
}
