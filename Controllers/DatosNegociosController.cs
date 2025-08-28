using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;

namespace RestauranteApp.Controllers
{
    public class DatosNegociosController : Controller
    {
        private readonly AppDbContext _context;

        public DatosNegociosController(AppDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Edit(int id, [Bind("DatosNegocioId,Nombre,Telefono,Ruc,DireccionNegocio")] DatosNegocio datosNegocio)
        {
            if (id != datosNegocio.DatosNegocioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
