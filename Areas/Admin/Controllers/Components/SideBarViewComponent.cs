using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Areas.Admin.Controllers.Components
{
    public class SideBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
