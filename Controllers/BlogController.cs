using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Controllers
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
