using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;
using System.Threading.Tasks;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManagersController : Controller
    {
        private readonly online_aptitude_testsContext _context;
        public ManagersController(online_aptitude_testsContext context)
        {
            _context = context;
        }
        // GET: ManagersController
        public async Task<IActionResult> Index()
        {
            var manager = await _context.Managers.ToListAsync();
            return View(manager);
        }

        // GET: ManagersController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: ManagersController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ManagersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Manager manager)
        {
            if(!ModelState.IsValid)
            {
                return View(manager);
            }
            try
            {
                var hasher = new PasswordHasher<Manager>();
                manager.PasswordHash = hasher.HashPassword(manager, manager.PasswordHash);

                _context.Managers.Add(manager);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Add successfuly";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception)
            {
                return View(manager);
            }
        }

        // GET: ManagersController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(e => e.Id == id);
            if(manager == null)
            {
                return NotFound("not found");
            }
            return View(manager);
        }

        // POST: ManagersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Manager manager)
        {
            if(id != manager.Id)
            {
                return NotFound();
            }
            if(!ModelState.IsValid)
            {
                return View(manager);
            }
            try
            {
                _context.Managers.Update(manager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "error"+ex.Message);
                return View(manager);
            }
        }

        // GET: ManagersController/Delete/5
        public IActionResult Delete(int id)
        {
            return View();
        }

        // POST: ManagersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
