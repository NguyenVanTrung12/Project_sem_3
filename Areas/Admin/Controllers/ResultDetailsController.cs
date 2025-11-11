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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Role_Supper_Managers,Role_Managers")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultDetail = await _context.ResultDetails
                .Include(d => d.Result)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (resultDetail == null)
            {
                TempData["Error"] = "Result detail not found.";
                return RedirectToAction("Index", "Results");
            }

            try
            {
                _context.ResultDetails.Remove(resultDetail);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Result detail deleted successfully!";
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("REFERENCE") == true)
            {
                TempData["Error"] = "❌ Cannot delete this result detail because it is referenced by other data.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ An error occurred while deleting the result detail: {ex.Message}";
            }

            return RedirectToAction("Details", "Results", new { id = resultDetail.ResultId });
        }
    }
}
