
using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult Edit() {
            return View();
        }
        public IActionResult List() {
            return View();
        }
    }
}

