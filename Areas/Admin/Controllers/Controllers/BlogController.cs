using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers.Controllers
{
    public class BlogController : Controller
    {
        [Route("blog.html")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
