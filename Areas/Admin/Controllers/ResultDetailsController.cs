using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Authorize]
    public class ResultDetailsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
