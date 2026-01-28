using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Express_Voitures.Data;
using Express_Voitures.Models;

namespace Express_Voitures.Controllers
{
    public class ModelesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModelesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Modeles (accessible à tous)
        public async Task<IActionResult> Index()
        {
            var modeles = await _context.Modeles
                .Include(m => m.Marque)
                .OrderBy(m => m.Marque.Nom)
                .ThenBy(m => m.Nom)
                .ToListAsync();
            return View(modeles);
        }

        // GET: Modeles/Details/5 (accessible à tous)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _context.Modeles
                .Include(m => m.Marque)
                .Include(m => m.Finitions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modele == null)
            {
                return NotFound();
            }

            return View(modele);
        }

        // GET: Modeles/Create (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["MarqueId"] = new SelectList(_context.Marques.OrderBy(m => m.Nom), "Id", "Nom");
            return View();
        }

        // POST: Modeles/Create (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Nom,MarqueId")] Modele modele)
        {
            if (await _context.Modeles.AnyAsync(m => m.Nom == modele.Nom && m.MarqueId == modele.MarqueId))
            {
                var marque = await _context.Marques.FindAsync(modele.MarqueId);
                ModelState.AddModelError("Nom", $"Le modèle '{modele.Nom}' existe déjà pour la marque {marque?.Nom}.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(modele);
                await _context.SaveChangesAsync();

                var marqueNom = (await _context.Marques.FindAsync(modele.MarqueId))?.Nom;
                TempData["SuccessMessage"] = $"Le modèle '{modele.Nom}' ({marqueNom}) a été créé avec succès.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MarqueId"] = new SelectList(_context.Marques.OrderBy(m => m.Nom), "Id", "Nom", modele.MarqueId);
            return View(modele);
        }

        // GET: Modeles/Edit/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _context.Modeles.FindAsync(id);
            if (modele == null)
            {
                return NotFound();
            }

            ViewData["MarqueId"] = new SelectList(_context.Marques.OrderBy(m => m.Nom), "Id", "Nom", modele.MarqueId);
            return View(modele);
        }

        // POST: Modeles/Edit/5 (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,MarqueId")] Modele modele)
        {
            if (id != modele.Id)
            {
                return NotFound();
            }

            if (await _context.Modeles.AnyAsync(m => m.Nom == modele.Nom && m.MarqueId == modele.MarqueId && m.Id != id))
            {
                var marque = await _context.Marques.FindAsync(modele.MarqueId);
                ModelState.AddModelError("Nom", $"Un autre modèle '{modele.Nom}' existe déjà pour la marque {marque?.Nom}.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modele);
                    await _context.SaveChangesAsync();

                    var marqueNom = (await _context.Marques.FindAsync(modele.MarqueId))?.Nom;
                    TempData["SuccessMessage"] = $"Le modèle '{modele.Nom}' ({marqueNom}) a été modifié avec succès.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModeleExists(modele.Id))
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

            ViewData["MarqueId"] = new SelectList(_context.Marques.OrderBy(m => m.Nom), "Id", "Nom", modele.MarqueId);
            return View(modele);
        }

        // GET: Modeles/Delete/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modele = await _context.Modeles
                .Include(m => m.Marque)
                .Include(m => m.Finitions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modele == null)
            {
                return NotFound();
            }

            return View(modele);
        }

        // POST: Modeles/Delete/5 (protégé - seul Jacques)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modele = await _context.Modeles
                .Include(m => m.Marque)
                .Include(m => m.Finitions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modele == null)
            {
                return NotFound();
            }

            if (modele.Finitions != null && modele.Finitions.Any())
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer le modèle '{modele.Nom}' ({modele.Marque?.Nom}) car il est utilisé par {modele.Finitions.Count} finition(s).";
                return RedirectToAction(nameof(Index));
            }

            var voituresUtilisant = await _context.Voitures.AnyAsync(v => v.ModeleId == id);
            if (voituresUtilisant)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer le modèle '{modele.Nom}' ({modele.Marque?.Nom}) car il est utilisé par des voitures.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Modeles.Remove(modele);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Le modèle '{modele.Nom}' ({modele.Marque?.Nom}) a été supprimé avec succès.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer le modèle '{modele.Nom}' car il est utilisé.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ModeleExists(int id)
        {
            return _context.Modeles.Any(e => e.Id == id);
        }
    }
}