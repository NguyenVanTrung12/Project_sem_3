using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers.Controllers
{
    public class ContactController : Controller
    {
        [Route("contact.html")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
