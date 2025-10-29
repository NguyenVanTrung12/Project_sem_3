using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers.Controllers
{
    public class AboutController : Controller
    {
        [Route("about.html")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
