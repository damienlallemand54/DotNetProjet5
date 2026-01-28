using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Express_Voitures.Data;
using Express_Voitures.Models;

namespace Express_Voitures.Controllers
{
    public class FinitionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinitionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Finitions (accessible à tous)
        public async Task<IActionResult> Index()
        {
            var finitions = await _context.Finitions
                .Include(f => f.Modele)
                    .ThenInclude(m => m.Marque)
                .OrderBy(f => f.Modele.Marque.Nom)
                .ThenBy(f => f.Modele.Nom)
                .ThenBy(f => f.Nom)
                .ToListAsync();
            return View(finitions);
        }

        // GET: Finitions/Details/5 (accessible à tous)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _context.Finitions
                .Include(f => f.Modele)
                    .ThenInclude(m => m.Marque)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (finition == null)
            {
                return NotFound();
            }

            return View(finition);
        }

        // GET: Finitions/Create (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var modeles = _context.Modeles
                .Include(m => m.Marque)
                .OrderBy(m => m.Marque.Nom)
                .ThenBy(m => m.Nom)
                .Select(m => new
                {
                    m.Id,
                    Display = $"{m.Marque.Nom} - {m.Nom}"
                })
                .ToList();

            ViewData["ModeleId"] = new SelectList(modeles, "Id", "Display");
            return View();
        }

        // POST: Finitions/Create (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Nom,ModeleId")] Finition finition)
        {
            // Vérifier si cette finition existe déjà pour ce modèle
            if (await _context.Finitions.AnyAsync(f => f.Nom == finition.Nom && f.ModeleId == finition.ModeleId))
            {
                var modele = await _context.Modeles
                    .Include(m => m.Marque)
                    .FirstOrDefaultAsync(m => m.Id == finition.ModeleId);
                ModelState.AddModelError("Nom", $"La finition '{finition.Nom}' existe déjà pour le modèle {modele?.Marque?.Nom} {modele?.Nom}.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(finition);
                await _context.SaveChangesAsync();

                var modele = await _context.Modeles
                    .Include(m => m.Marque)
                    .FirstOrDefaultAsync(m => m.Id == finition.ModeleId);
                TempData["SuccessMessage"] = $"La finition '{finition.Nom}' ({modele?.Marque?.Nom} {modele?.Nom}) a été créée avec succès.";
                return RedirectToAction(nameof(Index));
            }

            var modeles = _context.Modeles
                .Include(m => m.Marque)
                .OrderBy(m => m.Marque.Nom)
                .ThenBy(m => m.Nom)
                .Select(m => new
                {
                    m.Id,
                    Display = $"{m.Marque.Nom} - {m.Nom}"
                })
                .ToList();
            ViewData["ModeleId"] = new SelectList(modeles, "Id", "Display", finition.ModeleId);
            return View(finition);
        }

        // GET: Finitions/Edit/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _context.Finitions.FindAsync(id);
            if (finition == null)
            {
                return NotFound();
            }

            var modeles = _context.Modeles
                .Include(m => m.Marque)
                .OrderBy(m => m.Marque.Nom)
                .ThenBy(m => m.Nom)
                .Select(m => new
                {
                    m.Id,
                    Display = $"{m.Marque.Nom} - {m.Nom}"
                })
                .ToList();
            ViewData["ModeleId"] = new SelectList(modeles, "Id", "Display", finition.ModeleId);
            return View(finition);
        }

        // POST: Finitions/Edit/5 (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,ModeleId")] Finition finition)
        {
            if (id != finition.Id)
            {
                return NotFound();
            }

            if (await _context.Finitions.AnyAsync(f => f.Nom == finition.Nom && f.ModeleId == finition.ModeleId && f.Id != id))
            {
                var modele = await _context.Modeles
                    .Include(m => m.Marque)
                    .FirstOrDefaultAsync(m => m.Id == finition.ModeleId);
                ModelState.AddModelError("Nom", $"Une autre finition '{finition.Nom}' existe déjà pour le modèle {modele?.Marque?.Nom} {modele?.Nom}.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(finition);
                    await _context.SaveChangesAsync();

                    var modele = await _context.Modeles
                        .Include(m => m.Marque)
                        .FirstOrDefaultAsync(m => m.Id == finition.ModeleId);
                    TempData["SuccessMessage"] = $"La finition '{finition.Nom}' ({modele?.Marque?.Nom} {modele?.Nom}) a été modifiée avec succès.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinitionExists(finition.Id))
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

            var modeles = _context.Modeles
                .Include(m => m.Marque)
                .OrderBy(m => m.Marque.Nom)
                .ThenBy(m => m.Nom)
                .Select(m => new
                {
                    m.Id,
                    Display = $"{m.Marque.Nom} - {m.Nom}"
                })
                .ToList();
            ViewData["ModeleId"] = new SelectList(modeles, "Id", "Display", finition.ModeleId);
            return View(finition);
        }

        // GET: Finitions/Delete/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finition = await _context.Finitions
                .Include(f => f.Modele)
                    .ThenInclude(m => m.Marque)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (finition == null)
            {
                return NotFound();
            }

            return View(finition);
        }

        // POST: Finitions/Delete/5 (protégé - seul Jacques)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var finition = await _context.Finitions
                .Include(f => f.Modele)
                    .ThenInclude(m => m.Marque)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (finition == null)
            {
                return NotFound();
            }

            var voituresUtilisant = await _context.Voitures.AnyAsync(v => v.FinitionId == id);
            if (voituresUtilisant)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer la finition '{finition.Nom}' ({finition.Modele?.Marque?.Nom} {finition.Modele?.Nom}) car elle est utilisée par des voitures.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Finitions.Remove(finition);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"La finition '{finition.Nom}' ({finition.Modele?.Marque?.Nom} {finition.Modele?.Nom}) a été supprimée avec succès.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer la finition '{finition.Nom}' car elle est utilisée.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FinitionExists(int id)
        {
            return _context.Finitions.Any(e => e.Id == id);
        }
    }
}