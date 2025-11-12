using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;


using System.Text;
using X.PagedList.Extensions;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ReportController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public ReportController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        public IActionResult Index(string range, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            DateTime today = DateTime.Now;

            // ✅ Auto range selection
            if (!startDate.HasValue && !endDate.HasValue)
            {
                switch (range)
                {
                    case "today":
                        startDate = today.Date;
                        endDate = today;
                        break;

                    case "week":
                        startDate = today.AddDays(-7);
                        endDate = today;
                        break;

                    case "month":
                        startDate = today.AddMonths(-1);
                        endDate = today;
                        break;

                    default:
                        startDate = new DateTime(1753, 1, 1);
                        endDate = today;
                        break;
                }
            }

            // ✅ Fix SQL DateTime overflow
            startDate ??= new DateTime(1753, 1, 1);
            endDate ??= today;

            var query = _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .Where(r => r.SubmitDate >= startDate && r.SubmitDate <= endDate)
                .Select(r => new ReportViewModel
                {
                    CandidateId = r.CandidateId,
                    CandidateName = r.Candidate.Fullname,
                    SubjectName = r.Subject.SubjectName,
                    TotalMark = r.TotalMark ?? 0,
                    SubmitDate = r.SubmitDate ?? DateTime.UtcNow,
                    Passed = (r.TotalMark ?? 0) >= 2 // ✅ rule pass >= 12
                })
                .ToList();
            // ✅ Phân trang
            int pageSize = 10;
            var pagedList = query
                .OrderByDescending(x => x.SubmitDate)
                .ToPagedList(page, pageSize);

            // ✅ Dashboard summary
            ViewBag.TotalCandidates = _context.Candidates.Count();
            ViewBag.Appeared = query
            .Select(x => x.CandidateId)
            .Distinct()
            .Count();
            ViewBag.Passed = query.Count(x => x.Passed);

            // ✅ Pie chart data
            ViewBag.PassFailData = System.Text.Json.JsonSerializer.Serialize(new int[]
            {
                query.Count(x => x.Passed),
                query.Count(x => !x.Passed)
            });

            // ✅ Bar chart (avg score by subject)
            var subjectAvg = query
                .GroupBy(x => x.SubjectName)
                .Select(g => new { Subject = g.Key, Avg = g.Average(x => x.TotalMark) })
                .ToList();

            ViewBag.SubjectLabels = System.Text.Json.JsonSerializer.Serialize(subjectAvg.Select(x => x.Subject));
            ViewBag.SubjectScores = System.Text.Json.JsonSerializer.Serialize(subjectAvg.Select(x => x.Avg));

            // ✅ Line chart (test count by date)
            var trend = query
                .GroupBy(x => x.SubmitDate.Date)
                .Select(g => new { Date = g.Key.ToString("dd/MM"), Count = g.Count() })
                .ToList();

            ViewBag.Dates = System.Text.Json.JsonSerializer.Serialize(trend.Select(x => x.Date));
            ViewBag.TestCounts = System.Text.Json.JsonSerializer.Serialize(trend.Select(x => x.Count));

            return View(pagedList);
        }

        // ✅ Move candidate to HR round
        public IActionResult TransferToHr(int id)
        {
            var check = _context.Transfers.FirstOrDefault(t => t.CandidateId == id);

            if (check == null)
            {
                _context.Transfers.Add(new Transfer
                {
                    CandidateId = id,
                    TransferDate = DateTime.Now,
                    FromStage = "Online Test",
                    ToStage = "HR Interview"
                });

                _context.SaveChanges();
            }

            TempData["success"] = "Candidate moved to HR round!";
            return RedirectToAction("Index");
        }

        // ✅ Export CSV
        public IActionResult ExportCsv()
        {
            var data = _context.Results
                .Include(r => r.Candidate)
                .Include(r => r.Subject)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Candidate,Subject,Score,Date");

            foreach (var r in data)
            {
                sb.AppendLine($"{r.Candidate.Fullname},{r.Subject.SubjectName},{r.TotalMark},{r.SubmitDate}");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Report.csv");
        }

        // ✅ Export Excel (simple CSV readable by Excel)
        public IActionResult ExportExcel()
        {
            return ExportCsv();
        }
    }
}
