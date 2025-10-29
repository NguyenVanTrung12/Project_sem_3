
using Microsoft.AspNetCore.Mvc;


namespace Project_sem_3.Controllers.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {

            return View();
        }
    }
}
