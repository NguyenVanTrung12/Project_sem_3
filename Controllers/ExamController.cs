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
            int? candidateId = sessionCandidateId;

            var subjects = await _context.Subjects.OrderBy(s => s.Id).ToListAsync();
            var results = candidateId != null
                ? await _context.Results.Where(r => r.CandidateId == candidateId.Value).ToListAsync()
                : new List<Result>();

            bool blocked = false; // ✅ Nếu một vòng bị trượt, tất cả vòng sau bị khóa

            for (int i = 0; i < subjects.Count; i++)
            {
                var subject = subjects[i];
                var currentResult = results.FirstOrDefault(r => r.SubjectId == subject.Id);
                var prevResult = i > 0 ? results.FirstOrDefault(r => r.SubjectId == subjects[i - 1].Id) : null;

                bool isDone = currentResult?.Status == 1;   // Đã đậu
                bool isFailed = currentResult?.Status == 2; // Thi trượt
                bool canAccess = false;
                bool lockedDueToFail = false;

                // 🔒 Nếu vòng trước hoặc bất kỳ vòng nào trước đó đã trượt => khóa
                if (blocked)
                {
                    canAccess = false;
                    lockedDueToFail = true;
                }
                else
                {
                    if (i == 0)
                    {
                        // ✅ Vòng đầu tiên: mở nếu chưa trượt
                        canAccess = !isFailed;
                    }
                    else
                    {
                        // ✅ Các vòng sau: chỉ mở nếu vòng trước đậu và chưa trượt chính vòng này
                        canAccess = (prevResult != null && prevResult.Status == 1) && !isFailed;
                    }
                }

                // ❌ Nếu chính vòng hiện tại trượt => khóa tất cả vòng sau
                if (isFailed)
                {
                    blocked = true;
                    canAccess = false;
                    lockedDueToFail = true;
                }

                // 🔽 Truyền dữ liệu sang View
                ViewData[$"CanAccess_{subject.Id}"] = canAccess;
                ViewData[$"IsDone_{subject.Id}"] = isDone;
                ViewData[$"IsFailed_{subject.Id}"] = isFailed;
                ViewData[$"LockedDueToFail_{subject.Id}"] = lockedDueToFail;
            }

            ViewData["IsLoggedIn"] = candidateId != null;
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
                .AnyAsync(r => r.CandidateId == candidateId && r.TotalMark < 2 && r.Status == 1);
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
                q.Answers = q.Answers.OrderBy(a => Guid.NewGuid()).Take(4).ToList();
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

            // Nếu đã nộp rồi thì không cho nộp lại
            if (result.Status == 1 || result.Status == 2 || result.SubmitDate != null)
                return RedirectToAction("Result", new { id = resultId, pass = result.Status == 1 });

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

            // ✅ Cập nhật kết quả bài thi
            bool isPassed = totalMark >= 2;
            result.TotalMark = totalMark;
            result.SubmitDate = DateTime.Now;
            result.Status = isPassed ? 1 : 2; // 1 = Đậu, 2 = Trượt
            await _context.SaveChangesAsync();

            // Xóa session bài thi (tránh F5 thi lại)
            if (!string.IsNullOrEmpty(sessionKey))
                HttpContext.Session.Remove(sessionKey);

            // ✅ Nếu đậu phần cuối (Computer Technology) → chuyển sang HR Round
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
            // Phần 1 (Kiến thức chung) luôn mở
            if (subjectId == 1)
                return true;

            // Kiểm tra tất cả các phần trước
            for (int prevId = 1; prevId < subjectId; prevId++)
            {
                var prevResult = results.FirstOrDefault(r => r.SubjectId == prevId);

                // Nếu chưa có kết quả phần trước → khóa
                if (prevResult == null)
                    return false;

                // Nếu phần trước bị trượt (Status = 2) → khóa luôn phần này
                if (prevResult.Status == 2)
                    return false;

                // Nếu phần trước chưa hoàn thành hoặc trạng thái khác đậu → khóa
                if (prevResult.Status != 1)
                    return false;
            }

            // Nếu tất cả phần trước đều đã đậu → mở khóa phần này
            return true;
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