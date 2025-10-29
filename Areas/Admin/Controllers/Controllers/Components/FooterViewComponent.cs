using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers.Controllers.Components
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {

            return View();
        }
    }
}
