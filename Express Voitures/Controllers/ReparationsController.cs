using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Express_Voitures.Data;
using Express_Voitures.Models;

namespace Express_Voitures.Controllers
{
    public class ReparationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReparationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reparations/Create?voitureId=5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int? voitureId)
        {
            if (voitureId == null)
            {
                return NotFound();
            }

            var voiture = await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .FirstOrDefaultAsync(v => v.Id == voitureId);

            if (voiture == null)
            {
                return NotFound();
            }

            ViewData["Voiture"] = voiture;

            var reparation = new Reparation
            {
                VoitureId = voitureId.Value,
                DateReparation = DateTime.Now
            };

            return View(reparation);
        }

        // POST: Reparations/Create (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("VoitureId,Description,Cout,DateReparation")] Reparation reparation)
        {
            ModelState.Remove("Voiture");

            if (ModelState.IsValid)
            {
                _context.Add(reparation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"La réparation '{reparation.Description}' ({reparation.Cout:C}) a été ajoutée.";

                return RedirectToAction("Edit", "Voitures", new { id = reparation.VoitureId });
            }

            var voiture = await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .FirstOrDefaultAsync(v => v.Id == reparation.VoitureId);

            ViewData["Voiture"] = voiture;
            return View(reparation);
        }

        // GET: Reparations/Edit/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reparation = await _context.Reparations
                .Include(r => r.Voiture)
                    .ThenInclude(v => v.Marque)
                .Include(r => r.Voiture)
                    .ThenInclude(v => v.Modele)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reparation == null)
            {
                return NotFound();
            }

            ViewData["Voiture"] = reparation.Voiture;
            return View(reparation);
        }

        // POST: Reparations/Edit/5 (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VoitureId,Description,Cout,DateReparation")] Reparation reparation)
        {
            if (id != reparation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reparation);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"La réparation '{reparation.Description}' a été modifiée avec succès.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReparationExists(reparation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Voitures", new { id = reparation.VoitureId });
            }

            var voiture = await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .FirstOrDefaultAsync(v => v.Id == reparation.VoitureId);

            ViewData["Voiture"] = voiture;
            return View(reparation);
        }

        // GET: Reparations/Delete/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reparation = await _context.Reparations
                .Include(r => r.Voiture)
                    .ThenInclude(v => v.Marque)
                .Include(r => r.Voiture)
                    .ThenInclude(v => v.Modele)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reparation == null)
            {
                return NotFound();
            }

            return View(reparation);
        }

        // POST: Reparations/Delete/5 (protégé - seul Jacques)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reparation = await _context.Reparations.FindAsync(id);

            if (reparation == null) return NotFound();

            var voitureId = reparation.VoitureId;

            _context.Reparations.Remove(reparation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Réparation supprimée.";

            
            return RedirectToAction("Edit", "Voitures", new { id = voitureId });
        }

        private bool ReparationExists(int id)
        {
            return _context.Reparations.Any(e => e.Id == id);
        }
    }
}