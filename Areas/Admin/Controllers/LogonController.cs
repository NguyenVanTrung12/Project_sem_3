using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LogonController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
    }
}
