using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers
{
    public class QuestionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
