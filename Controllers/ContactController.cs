using Microsoft.AspNetCore.Mvc;
using Project_sem_3.Models;

namespace Project_sem_3.Controllers
{
    public class ContactController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public ContactController(online_aptitude_testsContext context)
        {
            _context = context;
        }
        [Route("contact.html")]
        public IActionResult Index()
        {
            var contact = _context.Contacts.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contact contact)
        {
            if(ModelState.IsValid)
            {
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                TempData["Success"] = "Contact sent successfully! We will respond as soon as possible.";
                return RedirectToAction(nameof(Index));
            }
                return View("Index",contact);
        }
    }
}
