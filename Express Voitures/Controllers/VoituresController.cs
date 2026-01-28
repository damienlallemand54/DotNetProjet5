using Express_Voitures.Data;
using Express_Voitures.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Express_Voitures.Controllers
{
    public class VoituresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public VoituresController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _environment = webHostEnvironment;
        }


        // GET: Voitures (accessible à tous)
        public async Task<IActionResult> Index(string statut = "disponibles")
        {
            var voitures = _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Reparations)
                .AsQueryable();

            // Filtrer selon le statut demandé
            voitures = statut.ToLower() switch
            {
                "disponibles" => voitures.Where(v => v.DateVente == null),
                "reparation" => voitures.Where(v => v.DateDisponibilite == null),
                "vendues" => voitures.Where(v => v.DateVente != null),
                "toutes" => voitures,
                _ => voitures.Where(v => v.DateDisponibilite != null && v.DateVente == null)
            };

            voitures = voitures.OrderByDescending(v => v.DateAchat);

            ViewData["StatutActuel"] = statut;
            return View(await voitures.ToListAsync());
        }

        // GET: Voitures/Details/5 (accessible à tous)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voiture = await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Reparations)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (voiture == null)
            {
                return NotFound();
            }

            return View(voiture);
        }

        // GET: Voitures/Create (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["MarqueId"] = new SelectList(_context.Marques.OrderBy(m => m.Nom), "Id", "Nom");
            // Les modèles et finitions seront chargés dynamiquement en JavaScript
            ViewData["ModeleId"] = new SelectList(Enumerable.Empty<Modele>(), "Id", "Nom");
            ViewData["FinitionId"] = new SelectList(Enumerable.Empty<Finition>(), "Id", "Nom");
            return View();
        }

        // POST: Voitures/Create (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Vin,Annee,MarqueId,ModeleId,FinitionId,DateAchat,PrixAchat,DateDisponibilite,DateVente,PhotoUrl,Description")] Voiture voiture, IFormFile? PhotoFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var err in errors)
                {
                    Console.WriteLine(err); // ou debugger
                }
            }
            else 
            {
                // Upload de l'image si un fichier a été choisi
                if (PhotoFile != null && PhotoFile.Length > 0)
                {
                    var folder = Path.Combine(_environment.WebRootPath, "images", "voitures");
                    Directory.CreateDirectory(folder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);
                    var filePath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await PhotoFile.CopyToAsync(stream);

                    voiture.PhotoUrl = "/images/voitures/" + fileName;
                }

                _context.Add(voiture);
                await _context.SaveChangesAsync();

                var marque = await _context.Marques.FindAsync(voiture.MarqueId);
                var modele = await _context.Modeles.FindAsync(voiture.ModeleId);
                TempData["SuccessMessage"] = $"La voiture {marque?.Nom} {modele?.Nom} ({voiture.Annee}) a été ajoutée avec succès.";
                return RedirectToAction("CreateSuccess");
            }

            ViewData["MarqueId"] = new SelectList(_context.Marques.OrderBy(m => m.Nom), "Id", "Nom", voiture.MarqueId);
            ViewData["ModeleId"] = new SelectList(_context.Modeles.Where(m => m.MarqueId == voiture.MarqueId).OrderBy(m => m.Nom), "Id", "Nom", voiture.ModeleId);
            ViewData["FinitionId"] = new SelectList(_context.Finitions.Where(f => f.ModeleId == voiture.ModeleId).OrderBy(f => f.Nom), "Id", "Nom", voiture.FinitionId);
            return View(voiture);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateSuccess()
        {
            return View();
        }


        // GET: Voitures/Edit/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var voiture = await _context.Voitures
                .Include(v => v.Marque)    // Ajouté
                .Include(v => v.Modele)    // Ajouté
                .Include(v => v.Finition)  // Ajouté
                .Include(v => v.Reparations)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (voiture == null) return NotFound();

            ViewData["MarqueId"] = new SelectList(_context.Marques.OrderBy(m => m.Nom), "Id", "Nom", voiture.MarqueId);
            ViewData["ModeleId"] = new SelectList(_context.Modeles.Where(m => m.MarqueId == voiture.MarqueId).OrderBy(m => m.Nom), "Id", "Nom", voiture.ModeleId);
            ViewData["FinitionId"] = new SelectList(_context.Finitions.Where(f => f.ModeleId == voiture.ModeleId).OrderBy(f => f.Nom), "Id", "Nom", voiture.FinitionId);

            return View(voiture);
        }

        //POST: Voitures/Edit/5 (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Vin,Annee,MarqueId,ModeleId,FinitionId,DateAchat,PrixAchat,DateDisponibilite,DateVente,PhotoUrl,Description,Reparations")] Voiture voiture, IFormFile? PhotoFile)
        {
            if (id != voiture.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (PhotoFile != null && PhotoFile.Length > 0)
                    {
                        var folder = Path.Combine(_environment.WebRootPath, "images", "voitures");
                        var fileName = Guid.NewGuid() + Path.GetExtension(PhotoFile.FileName);
                        var filePath = Path.Combine(folder, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await PhotoFile.CopyToAsync(stream);
                        voiture.PhotoUrl = "/images/voitures/" + fileName;
                    }

                   
                    _context.Update(voiture);

                   
                    if (voiture.Reparations != null)
                    {
                        foreach (var rep in voiture.Reparations)
                        {
                            
                            if (rep.Id == 0 && !string.IsNullOrWhiteSpace(rep.Description))
                            {
                                rep.VoitureId = voiture.Id;
                                _context.Reparations.Add(rep); 
                            }
                            
                            else if (rep.Id != 0)
                            {
                                _context.Reparations.Update(rep);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = voiture.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoitureExists(voiture.Id)) return NotFound();
                    else throw;
                }
            }

            ViewData["MarqueId"] = new SelectList(_context.Marques.OrderBy(m => m.Nom), "Id", "Nom", voiture.MarqueId);
            return View(voiture);
        }

        // GET: Voitures/Delete/5 (protégé - seul Jacques)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voiture = await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Reparations)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (voiture == null)
            {
                return NotFound();
            }

            return View(voiture);
        }

        
        // POST: Voitures/Delete/5 (protégé - seul Jacques)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voiture = await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .Include(v => v.Reparations) 
                .FirstOrDefaultAsync(v => v.Id == id);

            if (voiture == null)
            {
                return NotFound();
            }

            try
            {
                _context.Voitures.Remove(voiture);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Impossible de supprimer cette voiture.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View("DeleteSuccess", voiture);
        }


        // Action pour marquer une voiture comme disponible (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarquerDisponible(int id)
        {
            var voiture = await _context.Voitures.FindAsync(id);
            if (voiture == null)
            {
                return NotFound();
            }

            voiture.DateDisponibilite = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "La voiture a été marquée comme disponible.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Action pour marquer une voiture comme vendue (protégé - seul Jacques)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarquerVendue(int id)
        {
            var voiture = await _context.Voitures.FindAsync(id);
            if (voiture == null)
            {
                return NotFound();
            }

            if (!voiture.DateDisponibilite.HasValue)
            {
                TempData["ErrorMessage"] = "Une voiture doit être disponible avant d'être vendue.";
                return RedirectToAction(nameof(Details), new { id });
            }

            voiture.DateVente = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "La voiture a été marquée comme vendue.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // API pour charger les modèles d'une marque (pour les listes en cascade)
        [HttpGet]
        public async Task<JsonResult> GetModelesByMarque(int marqueId)
        {
            var modeles = await _context.Modeles
                .Where(m => m.MarqueId == marqueId)
                .OrderBy(m => m.Nom)
                .Select(m => new { m.Id, m.Nom })
                .ToListAsync();

            return Json(modeles);
        }

        // API pour charger les finitions d'un modèle (pour les listes en cascade)
        [HttpGet]
        public async Task<JsonResult> GetFinitionsByModele(int modeleId)
        {
            var finitions = await _context.Finitions
                .Where(f => f.ModeleId == modeleId)
                .OrderBy(f => f.Nom)
                .Select(f => new { f.Id, f.Nom })
                .ToListAsync();

            return Json(finitions);
        }

        private bool VoitureExists(int id)
        {
            return _context.Voitures.Any(e => e.Id == id);
        }
    }
}