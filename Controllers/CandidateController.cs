using Microsoft.AspNetCore.Mvc;

namespace Project_sem_3.Controllers
{
    public class CandidateController : Controller
    {
        [Route("candidate.html")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("candidate-result")]
        public IActionResult CandidateResult()
        {
            return View();
        }
      
    }
}
