using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestauranteApp.Models;

namespace RestauranteApp.Controllers
{
    public class ItemsMenuController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ItemsMenuController> _logger;

        public ItemsMenuController(AppDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<ItemsMenuController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // GET: ItemsMenu
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ItemsMenu.Include(i => i.CategoriaMenu);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ItemsMenu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenu = await _context.ItemsMenu
                .Include(i => i.CategoriaMenu)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (itemMenu == null)
            {
                return NotFound();
            }

            return View(itemMenu);
        }

        // GET: ItemsMenu/Create
        public IActionResult Create()
        {
            ViewBag.CategoriaId = new SelectList(_context.CategoriasMenu, "CategoriaId", "NombreCategoria");
            ViewData["Ingredientes"] = _context.Ingredientes
                .OrderBy(i => i.NombreIngrediente)
                .ToList();

            return View();
        }

        // POST: ItemsMenu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,NombreItem,Descripcion,Precio,CategoriaId,Estado,ItemMenuIngredientes")] ItemMenu itemMenu, IFormFile? imagenFile)
        {
            _logger.LogInformation("=== CREATE POST START ===");
            _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);
            _logger.LogInformation("ItemMenu received: NombreItem={NombreItem}, Precio={Precio}, CategoriaId={CategoriaId}, Estado={Estado}",
                itemMenu.NombreItem, itemMenu.Precio, itemMenu.CategoriaId, itemMenu.Estado);
            
            if (itemMenu.ItemMenuIngredientes != null)
            {
                _logger.LogInformation("ItemMenuIngredientes count: {Count}", itemMenu.ItemMenuIngredientes.Count);
                foreach (var ing in itemMenu.ItemMenuIngredientes)
                {
                    _logger.LogInformation("  Ingrediente: IngredienteId={IngredienteId}, Cantidad={Cantidad}, ItemMenuId={ItemMenuId}",
                        ing.IngredienteId, ing.Cantidad, ing.ItemMenuId);
                }
            }
            else
            {
                _logger.LogWarning("ItemMenuIngredientes is NULL");
            }

            // Log ModelState errors if any
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        _logger.LogError("ModelState Error - Key: {Key}, Error: {Error}", error.Key, err.ErrorMessage);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imagenFile != null && imagenFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "items");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imagenFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagenFile.CopyToAsync(fileStream);
                        }

                        itemMenu.ImagenUrl = "/images/items/" + uniqueFileName;
                        _logger.LogInformation("Image saved: {ImageUrl}", itemMenu.ImagenUrl);
                    }

                    if (itemMenu.ItemMenuIngredientes != null)
                    {
                        itemMenu.ItemMenuIngredientes = itemMenu.ItemMenuIngredientes
                            .Where(i => i.IngredienteId > 0 && i.Cantidad > 0)
                            .ToList();

                        foreach (var itemIngrediente in itemMenu.ItemMenuIngredientes)
                        {
                            itemIngrediente.ItemMenu = null;
                            itemIngrediente.Ingrediente = null;
                            _logger.LogInformation("Adding ingredient: IngredienteId={IngredienteId}, Cantidad={Cantidad}",
                                itemIngrediente.IngredienteId, itemIngrediente.Cantidad);
                        }
                    }

                    _logger.LogInformation("Adding ItemMenu to context...");
                    _context.Add(itemMenu);
                    
                    _logger.LogInformation("Calling SaveChangesAsync...");
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("SaveChangesAsync completed. ItemId: {ItemId}", itemMenu.ItemId);
                    
                    TempData["SuccessMessage"] = "¡Item de menú guardado exitosamente!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving ItemMenu: {Message}", ex.Message);
                    if (ex.InnerException != null)
                    {
                        _logger.LogError(ex.InnerException, "Inner exception: {Message}", ex.InnerException.Message);
                    }
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }

            ViewBag.CategoriaId = new SelectList(_context.CategoriasMenu, "CategoriaId", "NombreCategoria", itemMenu.CategoriaId);
            ViewData["Ingredientes"] = _context.Ingredientes
                .OrderBy(i => i.NombreIngrediente)
                .ToList();

            _logger.LogInformation("=== CREATE POST END (returning view) ===");
            return View(itemMenu);
        }

        // GET: ItemsMenu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenu = await _context.ItemsMenu.FindAsync(id);
            if (itemMenu == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriasMenu, "CategoriaId", "NombreCategoria", itemMenu.CategoriaId);
            return View(itemMenu);
        }

        // POST: ItemsMenu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,NombreItem,Descripcion,Precio,CategoriaId,Estado,ImagenUrl")] ItemMenu itemMenu, IFormFile? imagenFile)
        {
            if (id != itemMenu.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imagenFile != null && imagenFile.Length > 0)
                    {
                        // Eliminar imagen anterior si existe
                        if (!string.IsNullOrEmpty(itemMenu.ImagenUrl))
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, itemMenu.ImagenUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Guardar nueva imagen
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "items");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imagenFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imagenFile.CopyToAsync(fileStream);
                        }

                        itemMenu.ImagenUrl = "/images/items/" + uniqueFileName;
                    }

                    _context.Update(itemMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemMenuExists(itemMenu.ItemId))
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
            ViewData["CategoriaId"] = new SelectList(_context.CategoriasMenu, "CategoriaId", "NombreCategoria", itemMenu.CategoriaId);
            return View(itemMenu);
        }

        // GET: ItemsMenu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenu = await _context.ItemsMenu
                .Include(i => i.CategoriaMenu)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (itemMenu == null)
            {
                return NotFound();
            }

            return View(itemMenu);
        }

        // POST: ItemsMenu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemMenu = await _context.ItemsMenu.FindAsync(id);
            if (itemMenu != null)
            {
                // Eliminar imagen asociada si existe
                if (!string.IsNullOrEmpty(itemMenu.ImagenUrl))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, itemMenu.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.ItemsMenu.Remove(itemMenu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemMenuExists(int id)
        {
            return _context.ItemsMenu.Any(e => e.ItemId == id);
        }
    }
}
