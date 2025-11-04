using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project_sem_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_sem_3.Controllers
{
    public class ExamController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public ExamController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // 1️⃣ Trang chọn môn thi (General → Math → Computer)
        public async Task<IActionResult> Index()
        {
            var sessionCandidateId = HttpContext.Session.GetInt32("CandidateId");
            if (sessionCandidateId == null)
                return RedirectToAction("Index", "Logon");

            int candidateId = sessionCandidateId.Value;

            // Lấy danh sách 3 phần thi
            var subjects = await _context.Subjects.ToListAsync();

            // Lấy kết quả hiện tại của thí sinh
            var results = await _context.Results
                .Where(r => r.CandidateId == candidateId)
                .ToListAsync();

            // Xác định trạng thái mở/khóa từng phần
            foreach (var s in subjects)
            {
                bool canAccess = CanAccess(s.Id, results);
                ViewData[$"CanAccess_{s.Id}"] = canAccess;

                bool isDone = results.Any(r => r.SubjectId == s.Id && r.Status == 1);
                ViewData[$"IsDone_{s.Id}"] = isDone;
            }

            return View(subjects);
        }

        // 2️⃣ Bắt đầu thi
        public async Task<IActionResult> Start(int subjectId, int typeId = 1)
        {
            var sessionCandidateId = HttpContext.Session.GetInt32("CandidateId");
            if (sessionCandidateId == null)
                return RedirectToAction("Index", "Logon");

            int candidateId = sessionCandidateId.Value;

            // ⚠️ 1️⃣ Kiểm tra nếu thí sinh đã từng bị rớt ở phần thi trước
            var failedBefore = await _context.Results
                .AnyAsync(r => r.CandidateId == candidateId && r.TotalMark < 5 && r.Status == 1);
            if (failedBefore)
            {
                TempData["Error"] = "Bạn đã không đạt yêu cầu ở vòng trước, không thể tiếp tục thi.";
                return RedirectToAction("Index");
            }

            // 🔒 2️⃣ Kiểm tra quyền truy cập phần thi hiện tại (tuần tự)
            var allResults = await _context.Results
                .Where(r => r.CandidateId == candidateId)
                .ToListAsync();

            if (!CanAccess(subjectId, allResults))
            {
                TempData["Error"] = "Bạn chưa được mở khóa phần thi này!";
                return RedirectToAction("Index");
            }

            // 🔑 Session key duy nhất cho bài thi này
            string sessionKey = $"Exam_{candidateId}_{subjectId}_{typeId}";

            // ✅ Nếu Session đã có (F5 hoặc đang thi dở)
            var existingData = HttpContext.Session.GetString(sessionKey);
            if (!string.IsNullOrEmpty(existingData))
            {
                var savedData = JsonConvert.DeserializeObject<ExamSessionData>(existingData);
                ViewBag.ResultId = savedData.ResultId;
                ViewBag.CandidateId = candidateId;
                ViewBag.TypeId = typeId;
                ViewBag.TimeLimit = GetTimeLimit(subjectId);
                return View("Start", savedData.Questions);
            }

            // ✅ Tạo mới Result
            var result = new Result
            {
                CandidateId = candidateId,
                SubjectId = subjectId,
                TypeId = typeId,
                SubmitDate = null,
                Status = 0
            };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            // 🔀 Random 5 câu hỏi cho mỗi phần thi
            var questions = await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.SubjectId == subjectId)
                .OrderBy(q => Guid.NewGuid())
                .Take(5)
                .ToListAsync();

            foreach (var q in questions)
            {
                q.Answers = q.Answers.OrderBy(a => Guid.NewGuid()).ToList();
            }

            // 💾 Lưu vào Session
            var examData = new ExamSessionData
            {
                ResultId = result.Id,
                Questions = questions
            };
            HttpContext.Session.SetString(sessionKey, JsonConvert.SerializeObject(examData));

            ViewBag.ResultId = result.Id;
            ViewBag.CandidateId = candidateId;
            ViewBag.TypeId = typeId;
            ViewBag.TimeLimit = GetTimeLimit(subjectId);

            return View("Start", questions);
        }


        // 3️⃣ Nộp bài thi
        [HttpPost]
        public async Task<IActionResult> Submit(IFormCollection form)
        {
            var sessionCandidateId = HttpContext.Session.GetInt32("CandidateId");
            if (sessionCandidateId == null)
                return RedirectToAction("Index", "Logon");

            int candidateId = sessionCandidateId.Value;
            var sessionKey = form["sessionKey"].FirstOrDefault();
            var resultIdRaw = form["resultId"].FirstOrDefault();
            int.TryParse(resultIdRaw, out int resultId);

            // Nếu thiếu resultId → lấy lại từ session
            if (resultId <= 0 && !string.IsNullOrEmpty(sessionKey))
            {
                var data = HttpContext.Session.GetString(sessionKey);
                if (!string.IsNullOrEmpty(data))
                {
                    var saved = JsonConvert.DeserializeObject<ExamSessionData>(data);
                    if (saved != null) resultId = saved.ResultId;
                }
            }

            if (resultId <= 0) return BadRequest("Dữ liệu bài thi không hợp lệ.");

            var result = await _context.Results.FindAsync(resultId);
            if (result == null) return NotFound("Không tìm thấy bài thi.");
            if (result.CandidateId != candidateId) return Unauthorized();

            if (result.Status == 1 || result.SubmitDate != null)
                return RedirectToAction("Result", new { id = resultId, pass = result.TotalMark >= 5 });

            // Lấy danh sách câu trả lời
            var selectedAnswers = new Dictionary<int, int>();
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("selectedAnswers[") && form[key].Count > 0)
                {
                    var start = key.IndexOf('[') + 1;
                    var end = key.IndexOf(']');
                    if (start > 0 && end > start)
                    {
                        var qIdStr = key.Substring(start, end - start);
                        if (int.TryParse(qIdStr, out int qId) &&
                            int.TryParse(form[key].FirstOrDefault(), out int aId))
                        {
                            selectedAnswers[qId] = aId;
                        }
                    }
                }
            }

            double totalMark = 0;

            // Lưu kết quả từng câu
            foreach (var kv in selectedAnswers)
            {
                int questionId = kv.Key;
                int answerId = kv.Value;

                var answer = await _context.Answers.FindAsync(answerId);
                double mark = (answer != null && answer.Correctly == true) ? 1 : 0;
                totalMark += mark;

                var exist = await _context.ResultDetails
                    .FirstOrDefaultAsync(rd => rd.ResultId == resultId && rd.QuestionId == questionId);
                if (exist == null)
                {
                    _context.ResultDetails.Add(new ResultDetail
                    {
                        ResultId = resultId,
                        QuestionId = questionId,
                        AnswerId = answerId,
                        Mark = mark
                    });
                }
            }

            // Cập nhật Result
            result.TotalMark = totalMark;
            result.SubmitDate = DateTime.Now;
            result.Status = 1;
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(sessionKey))
                HttpContext.Session.Remove(sessionKey);

            bool isPassed = totalMark >= 5;

            // Nếu đã hoàn tất phần cuối (Computer Technology) → chuyển sang bảng Transfer
            if (isPassed && result.SubjectId == 3)
            {
                var transfer = new Transfer
                {
                    CandidateId = candidateId,
                    TransferDate = DateTime.Now,
                    FromStage = "Aptitude Test",
                    ToStage = "HR Round"
                };
                _context.Transfers.Add(transfer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Result", new { id = result.Id, pass = isPassed });
        }

        // 4️⃣ Trang xem kết quả
        public async Task<IActionResult> Result(int id, bool pass = false)
        {
            var sessionCandidateId = HttpContext.Session.GetInt32("CandidateId");
            if (sessionCandidateId == null)
                return RedirectToAction("Index", "Logon");

            int candidateId = sessionCandidateId.Value;

            var result = await _context.Results
                .Include(r => r.ResultDetails).ThenInclude(rd => rd.Question)
                .Include(r => r.ResultDetails).ThenInclude(rd => rd.Answer)
                .Include(r => r.Subject)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null) return NotFound();
            if (result.CandidateId != candidateId) return Unauthorized();

            ViewBag.IsPassed = pass;
            return View(result);
        }

        // ⚙️ Kiểm tra quyền truy cập phần thi
        private bool CanAccess(int subjectId, List<Result> results)
        {
            // General (1) luôn mở
            if (subjectId == 1) return true;

            // Math (2) chỉ khi General pass
            if (subjectId == 2)
                return results.Any(r => r.SubjectId == 1 && r.TotalMark >= 5);

            // Computer (3) chỉ khi Math pass
            if (subjectId == 3)
                return results.Any(r => r.SubjectId == 2 && r.TotalMark >= 5);

            return false;
        }

        // ⚙️ Giới hạn thời gian (phút)
        private int GetTimeLimit(int subjectId)
        {
            return subjectId switch
            {
                1 => 5,   // General Knowledge
                2 => 10,  // Mathematics
                3 => 15,  // Computer Technology
                _ => 10
            };
        }

        // Class phụ trợ lưu session
        private class ExamSessionData
        {
            public int ResultId { get; set; }
            public List<Question> Questions { get; set; } = new();
        }
    }
}
