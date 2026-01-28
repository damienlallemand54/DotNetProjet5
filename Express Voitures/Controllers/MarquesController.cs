using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Express_Voitures.Data;
using Express_Voitures.Models;

namespace Express_Voitures.Controllers
{
    public class MarquesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarquesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Marques (accessible à tous)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Marques.ToListAsync());
        }

        // GET: Marques/Details/5 (accessible à tous)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marque = await _context.Marques
                .Include(m => m.Modeles) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (marque == null)
            {
                return NotFound();
            }

            return View(marque);
        }

        // GET: Marques/Create (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marques/Create (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Nom")] Marque marque)
        {
            // Vérifier si la marque existe déjà
            if (await _context.Marques.AnyAsync(m => m.Nom == marque.Nom))
            {
                ModelState.AddModelError("Nom", "Cette marque existe déjà.");
                return View(marque);
            }

            if (ModelState.IsValid)
            {
                _context.Add(marque);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"La marque '{marque.Nom}' a été créée avec succès.";
                return RedirectToAction(nameof(Index));
            }
            return View(marque);
        }

        // GET: Marques/Edit/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marque = await _context.Marques.FindAsync(id);
            if (marque == null)
            {
                return NotFound();
            }
            return View(marque);
        }

        // POST: Marques/Edit/5 (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom")] Marque marque)
        {
            if (id != marque.Id)
            {
                return NotFound();
            }

            if (await _context.Marques.AnyAsync(m => m.Nom == marque.Nom && m.Id != id))
            {
                ModelState.AddModelError("Nom", "Une autre marque porte déjà ce nom.");
                return View(marque);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marque);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"La marque '{marque.Nom}' a été modifiée avec succès.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarqueExists(marque.Id))
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
            return View(marque);
        }

        // GET: Marques/Delete/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marque = await _context.Marques
                .Include(m => m.Modeles) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (marque == null)
            {
                return NotFound();
            }

            return View(marque);
        }

        // POST: Marques/Delete/5 (protégé - seul Jacques)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marque = await _context.Marques
                .Include(m => m.Modeles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (marque == null)
            {
                return NotFound();
            }

            if (marque.Modeles != null && marque.Modeles.Any())
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer la marque '{marque.Nom}' car elle est utilisée par {marque.Modeles.Count} modèle(s).";
                return RedirectToAction(nameof(Index));
            }

            var voituresUtilisant = await _context.Voitures.AnyAsync(v => v.MarqueId == id);
            if (voituresUtilisant)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer la marque '{marque.Nom}' car elle est utilisée par des voitures.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Marques.Remove(marque);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"La marque '{marque.Nom}' a été supprimée avec succès.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer la marque '{marque.Nom}' car elle est utilisée.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MarqueExists(int id)
        {
            return _context.Marques.Any(e => e.Id == id);
        }
    }
}