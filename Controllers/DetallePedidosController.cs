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
    public class DetallePedidosController : Controller
    {
        private readonly AppDbContext _context;

        public DetallePedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DetallePedidos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DetallesPedido.Include(d => d.ItemMenu).Include(d => d.Pedido);
            return View(await appDbContext.ToListAsync());
        }

        // GET: DetallePedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallesPedido
                .Include(d => d.ItemMenu)
                .Include(d => d.Pedido)
                .FirstOrDefaultAsync(m => m.DetalleId == id);
            if (detallePedido == null)
            {
                return NotFound();
            }

            return View(detallePedido);
        }

        // GET: DetallePedidos/Create
        public IActionResult Create()
        {
            ViewData["ItemId"] = new SelectList(_context.ItemsMenu, "ItemId", "NombreItem");
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "PedidoId", "EstadoPedido");
            return View();
        }

        // POST: DetallePedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DetalleId,PedidoId,ItemId,Cantidad,PrecioUnitario")] DetallePedido detallePedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallePedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.ItemsMenu, "ItemId", "NombreItem", detallePedido.ItemId);
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "PedidoId", "EstadoPedido", detallePedido.PedidoId);
            return View(detallePedido);
        }

        // GET: DetallePedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallesPedido.FindAsync(id);
            if (detallePedido == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.ItemsMenu, "ItemId", "NombreItem", detallePedido.ItemId);
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "PedidoId", "EstadoPedido", detallePedido.PedidoId);
            return View(detallePedido);
        }

        // POST: DetallePedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DetalleId,PedidoId,ItemId,Cantidad,PrecioUnitario")] DetallePedido detallePedido)
        {
            if (id != detallePedido.DetalleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallePedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallePedidoExists(detallePedido.DetalleId))
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
            ViewData["ItemId"] = new SelectList(_context.ItemsMenu, "ItemId", "NombreItem", detallePedido.ItemId);
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "PedidoId", "EstadoPedido", detallePedido.PedidoId);
            return View(detallePedido);
        }

        // GET: DetallePedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallesPedido
                .Include(d => d.ItemMenu)
                .Include(d => d.Pedido)
                .FirstOrDefaultAsync(m => m.DetalleId == id);
            if (detallePedido == null)
            {
                return NotFound();
            }

            return View(detallePedido);
        }

        // POST: DetallePedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detallePedido = await _context.DetallesPedido.FindAsync(id);
            if (detallePedido != null)
            {
                _context.DetallesPedido.Remove(detallePedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallePedidoExists(int id)
        {
            return _context.DetallesPedido.Any(e => e.DetalleId == id);
        }
    }
}
