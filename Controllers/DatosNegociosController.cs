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
using RestauranteApp.Models;

namespace RestauranteApp.Controllers
{
    public class DatosNegociosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DatosNegociosController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: DatosNegocios
        public async Task<IActionResult> Index()
        {
            return View(await _context.DatosNegocios.ToListAsync());
        }

        // GET: DatosNegocios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datosNegocio = await _context.DatosNegocios
                .FirstOrDefaultAsync(m => m.DatosNegocioId == id);
            if (datosNegocio == null)
            {
                return NotFound();
            }

            return View(datosNegocio);
        }

        // GET: DatosNegocios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DatosNegocios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatosNegocioId,Nombre,Telefono,Ruc,DireccionNegocio")] DatosNegocio datosNegocio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(datosNegocio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(datosNegocio);
        }

        // GET: DatosNegocios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datosNegocio = await _context.DatosNegocios.FindAsync(id);
            if (datosNegocio == null)
            {
                return NotFound();
            }
            return View(datosNegocio);
        }

        // POST: DatosNegocios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DatosNegocioId,Nombre,Telefono,Ruc,DireccionNegocio,Email")] DatosNegocio datosNegocio, IFormFile? logoFile)
        {
            if (id != datosNegocio.DatosNegocioId)
            {
                return NotFound();
            }

            // Obtener el registro actual para preservar LogoUrl si no se cambia
            var existingDatosNegocio = await _context.DatosNegocios.AsNoTracking().FirstOrDefaultAsync(d => d.DatosNegocioId == id);
            if (existingDatosNegocio == null)
            {
                return NotFound();
            }

            // Preservar LogoUrl si no se sube uno nuevo
            if (logoFile == null || logoFile.Length == 0)
            {
                datosNegocio.LogoUrl = existingDatosNegocio.LogoUrl;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (logoFile != null && logoFile.Length > 0)
                    {
                        // Eliminar logo anterior si existe
                        if (!string.IsNullOrEmpty(existingDatosNegocio.LogoUrl))
                        {
                            string oldLogoPath = Path.Combine(_webHostEnvironment.WebRootPath, existingDatosNegocio.LogoUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldLogoPath))
                            {
                                System.IO.File.Delete(oldLogoPath);
                            }
                        }

                        // Guardar nuevo logo
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logos");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + logoFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(fileStream);
                        }

                        datosNegocio.LogoUrl = "/images/logos/" + uniqueFileName;
                    }

                    _context.Update(datosNegocio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatosNegocioExists(datosNegocio.DatosNegocioId))
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
            return View(datosNegocio);
        }

        // GET: DatosNegocios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datosNegocio = await _context.DatosNegocios
                .FirstOrDefaultAsync(m => m.DatosNegocioId == id);
            if (datosNegocio == null)
            {
                return NotFound();
            }

            return View(datosNegocio);
        }

        // POST: DatosNegocios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datosNegocio = await _context.DatosNegocios.FindAsync(id);
            if (datosNegocio != null)
            {
                _context.DatosNegocios.Remove(datosNegocio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatosNegocioExists(int id)
        {
            return _context.DatosNegocios.Any(e => e.DatosNegocioId == id);
        }
    }
}
