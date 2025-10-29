using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;

namespace Project_sem_3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransfersController : Controller
    {
        private readonly online_aptitude_testsContext _context;

        public TransfersController(online_aptitude_testsContext context)
        {
            _context = context;
        }

        // GET: Transfers
        public IActionResult Index()
        {
            var transfers = _context.Transfers
                .Include(t => t.Candidate)
                .ToList();

            return View(transfers);
        }

        // GET: Transfers/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transfer = _context.Transfers
                .Include(t => t.Candidate)
                .FirstOrDefault(m => m.Id == id);

            if (transfer == null)
            {
                return NotFound();
            }

            return View(transfer);
        }

        // GET: Transfers/Create
        public IActionResult Create()
        {
            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Address");
            return View();
        }

        // POST: Transfers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,CandidateId,TransferDate,FromStage,ToStage")] Transfer transfer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transfer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Address", transfer.CandidateId);
            return View(transfer);
        }

        // GET: Transfers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transfer = _context.Transfers.Find(id);
            if (transfer == null)
            {
                return NotFound();
            }

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Address", transfer.CandidateId);
            return View(transfer);
        }

        // POST: Transfers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,CandidateId,TransferDate,FromStage,ToStage")] Transfer transfer)
        {
            if (id != transfer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transfer);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferExists(transfer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CandidateId"] = new SelectList(_context.Candidates, "Id", "Address", transfer.CandidateId);
            return View(transfer);
        }

        // GET: Transfers/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transfer = _context.Transfers
                .Include(t => t.Candidate)
                .FirstOrDefault(m => m.Id == id);

            if (transfer == null)
            {
                return NotFound();
            }

            return View(transfer);
        }

        // POST: Transfers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var transfer = _context.Transfers.Find(id);
            if (transfer != null)
            {
                _context.Transfers.Remove(transfer);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TransferExists(int id)
        {
            return _context.Transfers.Any(e => e.Id == id);
        }
    }
}
