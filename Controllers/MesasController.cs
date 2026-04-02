using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;
using RestauranteApp.ViewModels;

namespace RestauranteApp.Controllers
{
    public class MesasController : Controller
    {
        private readonly AppDbContext _context;

        public MesasController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> GestionVentaMesa()
        {
            // Obtener todas las mesas
            var mesas = await _context.Mesas.ToListAsync();
            
            // Obtener las facturas más recientes por mesa (solo las que tienen mesa asignada)
            var facturas = await _context.Facturas
                .Where(f => f.MesaId != null)
                .OrderByDescending(f => f.FechaPago)
                .ToListAsync();
            
            // Agrupar manualmente para obtener la factura más reciente por mesa
            var facturasActivas = facturas
                .GroupBy(f => f.MesaId!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            var vm = new GestionMesasVM
            {
                Mesas = mesas.Select(m => new MesaVentaVM
                {
                    MesaId = m.MesaId,
                    NumeroMesa = m.NumeroMesa,
                    Capacidad = m.Capacidad,
                    Estado = m.Estado,
                    FacturaId = facturasActivas.ContainsKey(m.MesaId) ? facturasActivas[m.MesaId].FacturaId : null,
                    MontoTotal = facturasActivas.ContainsKey(m.MesaId) ? facturasActivas[m.MesaId].MontoTotal : null,
                    MetodoPago = facturasActivas.ContainsKey(m.MesaId) ? facturasActivas[m.MesaId].MetodoPago : null,
                    FechaPago = facturasActivas.ContainsKey(m.MesaId) ? facturasActivas[m.MesaId].FechaPago : null
                }).ToList()
            };

            return View(vm);
        }

        // POST: Mesas/HacerOrden - Cambia el estado de la mesa a ocupada y redirige a crear factura
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HacerOrden(int mesaId)
        {
            var mesa = await _context.Mesas.FindAsync(mesaId);
            if (mesa == null)
            {
                return NotFound();
            }

            // Cambiar estado a ocupada
            mesa.Estado = "Ocupada";
            await _context.SaveChangesAsync();

            // Redirigir a crear factura con la mesa seleccionada
            return RedirectToAction("Create", "Facturas", new { mesaId = mesaId });
        }

        // POST: Mesas/CerrarVenta - Marca la venta como pagada y libera la mesa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarVenta(int mesaId)
        {
            var mesa = await _context.Mesas.FindAsync(mesaId);
            if (mesa == null)
            {
                return NotFound();
            }

            // Buscar la factura activa de esta mesa
            var factura = await _context.Facturas
                .Where(f => f.MesaId == mesaId)
                .OrderByDescending(f => f.FechaPago)
                .FirstOrDefaultAsync();

            if (factura != null)
            {
                // Marcar como pagada (podrías agregar un campo Estado a Factura o usar MetodoPago)
                // Por ahora solo liberamos la mesa
            }

            // Cambiar estado a disponible
            mesa.Estado = "Disponible";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"La mesa {mesa.NumeroMesa} ha sido liberada.";
            return RedirectToAction(nameof(GestionVentaMesa));
        }

        // POST: Mesas/CancelarVenta - Elimina la factura y libera la mesa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarVenta(int mesaId)
        {
            var mesa = await _context.Mesas.FindAsync(mesaId);
            if (mesa == null)
            {
                return NotFound();
            }

            // Buscar la factura activa de esta mesa
            var factura = await _context.Facturas
                .Include(f => f.DetalleFacturas)
                .Where(f => f.MesaId == mesaId)
                .OrderByDescending(f => f.FechaPago)
                .FirstOrDefaultAsync();

            if (factura != null)
            {
                // Eliminar los detalles de la factura
                if (factura.DetalleFacturas != null && factura.DetalleFacturas.Any())
                {
                    _context.DetalleFacturas.RemoveRange(factura.DetalleFacturas);
                }
                
                // Eliminar la factura
                _context.Facturas.Remove(factura);
                await _context.SaveChangesAsync();
            }

            // Cambiar estado a disponible
            mesa.Estado = "Disponible";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Venta cancelada. La mesa {mesa.NumeroMesa} está disponible.";
            return RedirectToAction(nameof(GestionVentaMesa));
        }

        // POST: Mesas/EditarVenta - Redirige a editar la factura de la mesa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarVenta(int mesaId)
        {
            var factura = await _context.Facturas
                .Where(f => f.MesaId == mesaId)
                .OrderByDescending(f => f.FechaPago)
                .FirstOrDefaultAsync();

            if (factura == null)
            {
                TempData["Error"] = "No se encontró una venta activa para esta mesa.";
                return RedirectToAction(nameof(GestionVentaMesa));
            }

            return RedirectToAction("Edit", "Facturas", new { id = factura.FacturaId });
        }


        // GET: Mesas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mesas.ToListAsync());
        }

        // GET: Mesas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesas
                .FirstOrDefaultAsync(m => m.MesaId == id);
            if (mesa == null)
            {
                return NotFound();
            }

            return View(mesa);
        }

        // GET: Mesas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mesas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MesaId,NumeroMesa,Capacidad,Estado")] Mesa mesa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mesa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mesa);
        }

        // GET: Mesas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa == null)
            {
                return NotFound();
            }
            return View(mesa);
        }

        // POST: Mesas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MesaId,NumeroMesa,Capacidad,Estado")] Mesa mesa)
        {
            if (id != mesa.MesaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mesa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MesaExists(mesa.MesaId))
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
            return View(mesa);
        }

        // GET: Mesas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesas
                .FirstOrDefaultAsync(m => m.MesaId == id);
            if (mesa == null)
            {
                return NotFound();
            }

            return View(mesa);
        }

        // POST: Mesas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa != null)
            {
                _context.Mesas.Remove(mesa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MesaExists(int id)
        {
            return _context.Mesas.Any(e => e.MesaId == id);
        }
    }
}
