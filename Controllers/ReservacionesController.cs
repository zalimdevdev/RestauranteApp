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
    public class ReservacionesController : Controller
    {
        private readonly AppDbContext _context;

        public ReservacionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reservaciones
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Reservaciones.Include(r => r.Cliente).Include(r => r.Mesa);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Reservaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservacion = await _context.Reservaciones
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.ReservacionId == id);
            if (reservacion == null)
            {
                return NotFound();
            }

            return View(reservacion);
        }

        // GET: Reservaciones/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Apellido");
            ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "Estado");
            return View();
        }

        // POST: Reservaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservacionId,ClienteId,MesaId,FechaHora,NumeroPersonas,Estado")] Reservacion reservacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Apellido", reservacion.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "Estado", reservacion.MesaId);
            return View(reservacion);
        }

        // GET: Reservaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Apellido", reservacion.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "Estado", reservacion.MesaId);
            return View(reservacion);
        }

        // POST: Reservaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservacionId,ClienteId,MesaId,FechaHora,NumeroPersonas,Estado")] Reservacion reservacion)
        {
            if (id != reservacion.ReservacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservacionExists(reservacion.ReservacionId))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Apellido", reservacion.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas, "MesaId", "Estado", reservacion.MesaId);
            return View(reservacion);
        }

        // GET: Reservaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservacion = await _context.Reservaciones
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.ReservacionId == id);
            if (reservacion == null)
            {
                return NotFound();
            }

            return View(reservacion);
        }

        // POST: Reservaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion != null)
            {
                _context.Reservaciones.Remove(reservacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservacionExists(int id)
        {
            return _context.Reservaciones.Any(e => e.ReservacionId == id);
        }
    }
}
