using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;
using RestauranteApp.ViewModels;
using System.Linq;

namespace RestauranteApp.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly AppDbContext _context;

        public EstadisticasController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? mes, int? anio)
        {
            // Si no se especifica mes/año, usar el mes actual
            mes ??= DateTime.Now.Month;
            anio ??= DateTime.Now.Year;

            var estadisticasVM = new EstadisticasVM
            {
                MesSeleccionado = mes.Value,
                AnioSeleccionado = anio.Value
            };

            // Consultar ventas del mes seleccionado
            var ventas = _context.DetalleFacturas
                .Include(df => df.ItemMenu)
                .Include(df => df.Factura)
                .Where(df => df.Factura.FechaPago.Month == mes && df.Factura.FechaPago.Year == anio)
                .GroupBy(df => df.ItemId)
                .Select(g => new ProductoVentaVM
                {
                    ItemId = g.Key,
                    NombreItem = g.First().ItemMenu.NombreItem,
                    TotalVendido = g.Sum(df => df.Cantidad),
                    TotalIngresos = g.Sum(df => df.Subtotal)
                })
                .OrderByDescending(v => v.TotalVendido)
                .ToList();

            if (ventas.Any())
            {
                estadisticasVM.ProductosMasVendidos = ventas.Take(10);
                estadisticasVM.ProductosMenosVendidos = ventas.OrderBy(v => v.TotalVendido).Take(10);
            }

            return View(estadisticasVM);
        }
    }
}