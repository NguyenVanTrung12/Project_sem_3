using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CandidatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
    }
}
