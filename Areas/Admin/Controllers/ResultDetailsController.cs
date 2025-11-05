using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
    public class ResultDetailsController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public ResultDetailsController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _context.ResultDetails
                .Include(d => d.Result)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detail == null)
            {
                TempData["Error"] = "Không tìm thấy chi tiết kết quả để xóa.";
                return RedirectToAction("Index", "Results");
            }

            try
            {
                _context.ResultDetails.Remove(detail);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Xóa chi tiết kết quả thành công!";
                return RedirectToAction("Details", "Results", new { id = detail.ResultId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Lỗi khi xóa chi tiết kết quả: {ex.Message}";
                return RedirectToAction("Details", "Results", new { id = detail.ResultId });
            }
        }

        // thêm asp-action="ConfirmDelete" vào form xóa trong View để gọi action này
        [HttpGet, ActionName("ConfirmDelete")]
        [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var resultDetail = await _context.ResultDetails.FindAsync(id);
            if (resultDetail == null)
            {
                TempData["Error"] = "Result Detail not found.";
                return NotFound();
            }

            bool hasRelations = await _context.ResultDetails.AnyAsync(r => r.AnswerId == id);
            if (hasRelations)
            {
                TempData["Error"] = "Cannot delete this Result Detail because it is associated with existing Results.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.ResultDetails.Remove(resultDetail);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Result Detail deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while deleting the Result Detail: {ex.Message}";
            }
            
            return RedirectToAction("Index");
        }
    }
}
