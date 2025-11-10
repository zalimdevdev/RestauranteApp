using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;
using RestauranteApp.ViewModels; // <- ViewModel
using Rotativa.AspNetCore;

namespace RestauranteApp.Controllers
{
    public class FacturasController : Controller
    {
        private readonly AppDbContext _context;

        public FacturasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Facturas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Mesa);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var factura = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Mesa)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(d => d.ItemMenu)
                .FirstOrDefaultAsync(m => m.FacturaId == id);

            if (factura == null) return NotFound();

            return View(factura);
        }

        // GET: Facturas/Create
        public async Task<IActionResult> Create()
        {
            // Obtener items activos (ItemMenu) con su precio
            var items = await _context.ItemsMenu
                .Where(i => i.Estado == "Activo")
                .Select(i => new { i.ItemId, i.NombreItem, i.Precio })
                .ToListAsync();

            var vm = new FacturaCreateVM
            {
                Items = items.Select(i => new SelectListItem { Value = i.ItemId.ToString(), Text = i.NombreItem }),
                ItemPrecios = items.ToDictionary(i => i.ItemId, i => i.Precio),
                Clientes = await _context.Clientes
                    .Select(c => new SelectListItem { Value = c.ClienteId.ToString(), Text = (c.Nombre ?? "") + " " + (c.Apellido ?? "") })
                    .ToListAsync(),
                Mesas = await _context.Mesas
                    .Select(m => new SelectListItem { Value = m.MesaId.ToString(), Text = m.NumeroMesa.ToString() })
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: Facturas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacturaCreateVM vm)
        {
            // Validación: al menos un detalle
            if (vm.Detalles == null || vm.Detalles.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Agrega al menos un producto.");
            }

            if (!ModelState.IsValid)
            {
                // Rehidratar listas si hay error de validación
                var itemsRe = await _context.ItemsMenu
                    .Where(i => i.Estado == "Activo")
                    .Select(i => new { i.ItemId, i.NombreItem, i.Precio })
                    .ToListAsync();

                vm.Items = itemsRe.Select(i => new SelectListItem { Value = i.ItemId.ToString(), Text = i.NombreItem });
                vm.ItemPrecios = itemsRe.ToDictionary(i => i.ItemId, i => i.Precio);
                vm.Clientes = await _context.Clientes
                    .Select(c => new SelectListItem { Value = c.ClienteId.ToString(), Text = (c.Nombre ?? "") + " " + (c.Apellido ?? "") })
                    .ToListAsync();
                vm.Mesas = await _context.Mesas
                    .Select(m => new SelectListItem { Value = m.MesaId.ToString(), Text = m.NumeroMesa.ToString() })
                    .ToListAsync();

                return View(vm);
            }

            // Recalcular precios en servidor (seguridad)
            var ids = vm.Detalles.Where(d => d.ItemId.HasValue).Select(d => d.ItemId.Value).Distinct().ToList();
            var priceMap = await _context.ItemsMenu
                .Where(i => ids.Contains(i.ItemId))
                .ToDictionaryAsync(i => i.ItemId, i => i.Precio);

            decimal total = 0m;
            var detalles = new List<DetalleFactura>();

            foreach (var d in vm.Detalles)
            {
                if (d.ItemId == null || !priceMap.TryGetValue(d.ItemId.Value, out var precio))
                {
                    ModelState.AddModelError(string.Empty, "Producto no válido.");
                    // rehidratar y devolver vista
                    var itemsRe = await _context.ItemsMenu
                        .Where(i => i.Estado == "Activo")
                        .Select(i => new { i.ItemId, i.NombreItem, i.Precio })
                        .ToListAsync();

                    vm.Items = itemsRe.Select(i => new SelectListItem { Value = i.ItemId.ToString(), Text = i.NombreItem });
                    vm.ItemPrecios = itemsRe.ToDictionary(i => i.ItemId, i => i.Precio);
                    vm.Clientes = await _context.Clientes
                        .Select(c => new SelectListItem { Value = c.ClienteId.ToString(), Text = (c.Nombre ?? "") + " " + (c.Apellido ?? "") })
                        .ToListAsync();
                    vm.Mesas = await _context.Mesas
                        .Select(m => new SelectListItem { Value = m.MesaId.ToString(), Text = m.NumeroMesa.ToString() })
                        .ToListAsync();

                    return View(vm);
                }

                var cantidad = d.Cantidad < 1 ? 1 : d.Cantidad;
                var subtotal = precio * cantidad;
                detalles.Add(new DetalleFactura
                {
                    ItemId = d.ItemId.Value,
                    Cantidad = cantidad,
                    PrecioUnitario = precio,
                    Subtotal = subtotal
                });
                total += subtotal;
            }

            // Crear factura (encabezado)
            var factura = new Factura
            {
                ClienteId = vm.ClienteId,
                MesaId = vm.MesaId,
                MetodoPago = vm.MetodoPago,
                FechaPago = vm.FechaPago ?? DateTime.Now,
                MontoTotal = total
            };

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            // asignar facturaId a cada detalle y guardar
            foreach (var det in detalles) det.FacturaId = factura.FacturaId;
            _context.DetalleFacturas.AddRange(detalles);
            await _context.SaveChangesAsync();

            // Retornar JSON con el ID de la factura para AJAX
            return Json(new { success = true, facturaId = factura.FacturaId });
        }

        // GET: Facturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var factura = await _context.Facturas
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(d => d.ItemMenu)
                .FirstOrDefaultAsync(f => f.FacturaId == id);

            if (factura == null) return NotFound();

            // Obtener items activos
            var items = await _context.ItemsMenu
                .Where(i => i.Estado == "Activo")
                .Select(i => new { i.ItemId, i.NombreItem, i.Precio })
                .ToListAsync();

            var vm = new FacturaCreateVM
            {
                FacturaId = factura.FacturaId,
                ClienteId = factura.ClienteId,
                MesaId = factura.MesaId,
                MetodoPago = factura.MetodoPago,
                FechaPago = factura.FechaPago,
                Items = items.Select(i => new SelectListItem { Value = i.ItemId.ToString(), Text = i.NombreItem }),
                ItemPrecios = items.ToDictionary(i => i.ItemId, i => i.Precio),
                Clientes = await _context.Clientes
                    .Select(c => new SelectListItem { Value = c.ClienteId.ToString(), Text = (c.Nombre ?? "") + " " + (c.Apellido ?? "") })
                    .ToListAsync(),
                Mesas = await _context.Mesas
                    .Select(m => new SelectListItem { Value = m.MesaId.ToString(), Text = m.NumeroMesa.ToString() })
                    .ToListAsync(),
                Detalles = factura.DetalleFacturas.Select(d => new FacturaDetalleVM
                {
                    ItemId = d.ItemId,
                    Cantidad = d.Cantidad
                }).ToList()
            };

            // Pasar detalles existentes al JS
            ViewBag.FacturaId = factura.FacturaId;
            var detalles = new List<Dictionary<string, object>>();
            if (factura.DetalleFacturas != null)
            {
                foreach (var d in factura.DetalleFacturas)
                {
                    var obj = new Dictionary<string, object>
                    {
                        ["itemId"] = d.ItemId.ToString(),
                        ["cantidad"] = d.Cantidad
                    };
                    detalles.Add(obj);
                }
            }
            ViewBag.ExistingDetails = detalles;

            return View(vm);
        }

        // POST: Facturas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FacturaCreateVM vm)
        {
            if (id != vm.FacturaId) return NotFound();

            // Validación: al menos un detalle
            if (vm.Detalles == null || vm.Detalles.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Agrega al menos un producto.");
            }

            if (!ModelState.IsValid)
            {
                // Rehidratar listas si hay error de validación
                var itemsRe = await _context.ItemsMenu
                    .Where(i => i.Estado == "Activo")
                    .Select(i => new { i.ItemId, i.NombreItem, i.Precio })
                    .ToListAsync();

                vm.Items = itemsRe.Select(i => new SelectListItem { Value = i.ItemId.ToString(), Text = i.NombreItem });
                vm.ItemPrecios = itemsRe.ToDictionary(i => i.ItemId, i => i.Precio);
                vm.Clientes = await _context.Clientes
                    .Select(c => new SelectListItem { Value = c.ClienteId.ToString(), Text = (c.Nombre ?? "") + " " + (c.Apellido ?? "") })
                    .ToListAsync();
                vm.Mesas = await _context.Mesas
                    .Select(m => new SelectListItem { Value = m.MesaId.ToString(), Text = m.NumeroMesa.ToString() })
                    .ToListAsync();

                ViewBag.FacturaId = vm.FacturaId;
                var detalles = new List<Dictionary<string, object>>();
                if (vm.Detalles != null)
                {
                    foreach (var detalle in vm.Detalles)
                    {
                        var obj = new Dictionary<string, object>
                        {
                            ["itemId"] = detalle.ItemId?.ToString(),
                            ["cantidad"] = detalle.Cantidad
                        };
                        detalles.Add(obj);
                    }
                }
                ViewBag.ExistingDetails = detalles;

                return View(vm);
            }

            // Recalcular precios en servidor (seguridad)
            var ids = vm.Detalles.Where(det => det.ItemId.HasValue).Select(det => det.ItemId.Value).Distinct().ToList();
            var priceMap = await _context.ItemsMenu
                .Where(i => ids.Contains(i.ItemId))
                .ToDictionaryAsync(i => i.ItemId, i => i.Precio);

            decimal total = 0m;
            var nuevosDetalles = new List<DetalleFactura>();

            foreach (var d in vm.Detalles)
            {
                if (d.ItemId == null || !priceMap.TryGetValue(d.ItemId.Value, out var precio))
                {
                    ModelState.AddModelError(string.Empty, "Producto no válido.");
                    // rehidratar y devolver vista
                    var itemsRe = await _context.ItemsMenu
                        .Where(i => i.Estado == "Activo")
                        .Select(i => new { i.ItemId, i.NombreItem, i.Precio })
                        .ToListAsync();

                    vm.Items = itemsRe.Select(i => new SelectListItem { Value = i.ItemId.ToString(), Text = i.NombreItem });
                    vm.ItemPrecios = itemsRe.ToDictionary(i => i.ItemId, i => i.Precio);
                    vm.Clientes = await _context.Clientes
                        .Select(c => new SelectListItem { Value = c.ClienteId.ToString(), Text = (c.Nombre ?? "") + " " + (c.Apellido ?? "") })
                        .ToListAsync();
                    vm.Mesas = await _context.Mesas
                        .Select(m => new SelectListItem { Value = m.MesaId.ToString(), Text = m.NumeroMesa.ToString() })
                        .ToListAsync();

                    ViewBag.FacturaId = vm.FacturaId;
                    var detalles = new List<Dictionary<string, object>>();
                    if (vm.Detalles != null)
                    {
                        foreach (var detalle in vm.Detalles)
                        {
                            var obj = new Dictionary<string, object>
                            {
                                ["itemId"] = detalle.ItemId?.ToString(),
                                ["cantidad"] = detalle.Cantidad
                            };
                            detalles.Add(obj);
                        }
                    }
                    ViewBag.ExistingDetails = detalles;

                    return View(vm);
                }

                var cantidad = d.Cantidad < 1 ? 1 : d.Cantidad;
                var subtotal = precio * cantidad;
                nuevosDetalles.Add(new DetalleFactura
                {
                    FacturaId = vm.FacturaId,
                    ItemId = d.ItemId.Value,
                    Cantidad = cantidad,
                    PrecioUnitario = precio,
                    Subtotal = subtotal
                });
                total += subtotal;
            }

            try
            {
                // Actualizar factura principal
                var factura = await _context.Facturas.FindAsync(vm.FacturaId);
                if (factura == null) return NotFound();

                factura.ClienteId = vm.ClienteId;
                factura.MesaId = vm.MesaId;
                factura.MetodoPago = vm.MetodoPago;
                factura.FechaPago = vm.FechaPago ?? DateTime.Now;
                factura.MontoTotal = total;

                // Eliminar detalles existentes y agregar nuevos
                var detallesExistentes = _context.DetalleFacturas.Where(d => d.FacturaId == vm.FacturaId);
                _context.DetalleFacturas.RemoveRange(detallesExistentes);
                _context.DetalleFacturas.AddRange(nuevosDetalles);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(vm.FacturaId)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Details), new { id = vm.FacturaId });
        }

        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var factura = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Mesa)
                .FirstOrDefaultAsync(m => m.FacturaId == id);

            if (factura == null) return NotFound();

            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
                // Si configuraste cascade delete en DetalleFactura, se eliminarán automáticamente.
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Facturas/FacturaPdf/5
        public async Task<IActionResult> FacturaPdf(int id)
        {
            var factura = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.Mesa)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(d => d.ItemMenu)
                .FirstOrDefaultAsync(f => f.FacturaId == id);

            if (factura == null) return NotFound();

            // Obtener datos del negocio para el PDF
            var datosNegocio = await _context.DatosNegocios.FirstOrDefaultAsync();
            ViewBag.DatosNegocio = datosNegocio;

            return new ViewAsPdf("FacturaPdf", factura);
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.FacturaId == id);
        }
    }
}
