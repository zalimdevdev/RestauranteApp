using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;
using RestauranteApp.Services;
using RestauranteApp.ViewModels;

namespace RestauranteApp.Controllers
{
    public class ReabastecimientosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IStockService _stockService;

        public ReabastecimientosController(AppDbContext context, IStockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        // GET: Reabastecimientos
        public async Task<IActionResult> Index()
        {
            var ingredientes = await _context.Ingredientes
                .Include(i => i.MovimientosStock)
                .OrderBy(i => i.NombreIngrediente)
                .ToListAsync();
            
            return View(ingredientes);
        }

        // GET: Reabastecimientos/Create
        public async Task<IActionResult> Create()
        {
            var ingredientes = await _context.Ingredientes
                .OrderBy(i => i.NombreIngrediente)
                .ToListAsync();

            // Preparar datos para el dropdown
            var ingredientesSelectList = ingredientes.Select(i => new SelectListItem
            {
                Value = i.IngredienteId.ToString(),
                Text = i.NombreIngrediente
            });

            // Preparar datos para mostrar información del ingrediente
            var ingredientesData = ingredientes.Select(i => new
            {
                value = i.IngredienteId,
                nombre = i.NombreIngrediente,
                unidadMedida = i.UnidadMedida,
                cantidadStock = i.CantidadStock
            });

            ViewBag.Ingredientes = ingredientesSelectList;
            ViewBag.IngredientesData = ingredientesData;

            var vm = new ReabastecimientoVM();

            return View(vm);
        }

        // POST: Reabastecimientos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReabastecimientoVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Ingredientes = await _context.Ingredientes
                    .OrderBy(i => i.NombreIngrediente)
                    .Select(i => new SelectListItem
                    {
                        Value = i.IngredienteId.ToString(),
                        Text = i.NombreIngrediente
                    })
                    .ToListAsync();
                
                return View(vm);
            }

            // Obtener el ingrediente
            var ingrediente = await _context.Ingredientes.FindAsync(vm.IngredienteId);
            if (ingrediente == null)
            {
                ModelState.AddModelError(string.Empty, "Ingrediente no encontrado.");
                vm.Ingredientes = await _context.Ingredientes
                    .OrderBy(i => i.NombreIngrediente)
                    .Select(i => new SelectListItem
                    {
                        Value = i.IngredienteId.ToString(),
                        Text = i.NombreIngrediente
                    })
                    .ToListAsync();
                
                return View(vm);
            }

            // Actualizar el stock
            ingrediente.CantidadStock += vm.Cantidad;

            // Registrar el movimiento
            var movimiento = new MovimientoStock
            {
                IngredienteId = vm.IngredienteId,
                Tipo = "Entrada",
                Cantidad = vm.Cantidad,
                Fecha = vm.Fecha,
                Observacion = vm.Observacion ?? "Reabastecimiento"
            };

            _context.MovimientosStock.Add(movimiento);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"¡Stock de {ingrediente.NombreIngrediente} actualizado exitosamente! Nuevo stock: {ingrediente.CantidadStock} {ingrediente.UnidadMedida}";
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Reabastecimientos/Historial/5
        public async Task<IActionResult> Historial(int id)
        {
            var ingrediente = await _context.Ingredientes
                .Include(i => i.MovimientosStock)
                .FirstOrDefaultAsync(i => i.IngredienteId == id);

            if (ingrediente == null)
            {
                return NotFound();
            }

            var movimientos = await _context.MovimientosStock
                .Where(m => m.IngredienteId == id)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            var vm = new DetalleMovimientoVM
            {
                IngredienteId = id,
                NombreIngrediente = ingrediente.NombreIngrediente,
                StockActual = ingrediente.CantidadStock,
                Movimientos = movimientos.Select(m => new MovimientoCantidadVM
                {
                    MovimientoId = m.MovimientoId,
                    Tipo = m.Tipo,
                    Cantidad = m.Cantidad,
                    Fecha = m.Fecha,
                    Observacion = m.Observacion,
                    FacturaId = m.FacturaId
                }).ToList()
            };

            return View(vm);
        }

        private bool IngredienteExists(int id)
        {
            return _context.Ingredientes.Any(e => e.IngredienteId == id);
        }
    }
}
