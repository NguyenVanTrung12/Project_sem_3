using Microsoft.AspNetCore.Mvc;
using Project_sem_3.Models;
using System.Linq;

namespace Project_sem_3.Controllers
{
    public class TransferController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public TransferController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // Trang danh sách hoặc hiển thị sau khi đậu vòng 3
        public IActionResult Index()
        {
            var result = new Result
            {
                CandidateId = 123,
                SubjectId = 3,
                SubmitDate = DateTime.Now,
                Subject = new Subject { SubjectName = "Computer Technology" }
            };

            ViewBag.IsPassed = true;
            return View(result);
        }

        // Trang xem chi tiết HR Round
        public IActionResult Details(int candidateId)
        {
            var transfer = _context.Transfers
                .Where(t => t.CandidateId == candidateId)
                .Select(t => new Transfer
                {
                    Id = t.Id,
                    CandidateId = t.CandidateId,
                    TransferDate = t.TransferDate,
                    FromStage = t.FromStage,
                    ToStage = t.ToStage,
                    Candidate = t.Candidate
                })
                .FirstOrDefault();

            if (transfer == null)
            {
                return NotFound();
            }

            return View(transfer);
        }
    }
}
