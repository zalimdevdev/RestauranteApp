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

            // Obtener datos para gráficos
            estadisticasVM.VentasDiarias = ObtenerVentasDiarias();
            estadisticasVM.VentasSemanales = ObtenerVentasSemanales();
            estadisticasVM.VentasMensuales = ObtenerVentasMensuales();

            return View(estadisticasVM);
        }

        private List<DatosGraficoVM> ObtenerVentasDiarias()
        {
            var hoy = DateTime.Today;
            var facturasHoy = _context.Facturas
                .Where(f => f.FechaPago.Date == hoy)
                .ToList();

            var result = new List<DatosGraficoVM>();

            // Crear un punto para el día actual
            result.Add(new DatosGraficoVM
            {
                Etiqueta = hoy.ToString("dd/MM/yyyy"),
                CantidadVentas = facturasHoy.Count,
                Ingresos = facturasHoy.Sum(f => f.MontoTotal)
            });

            return result;
        }

        private List<DatosGraficoVM> ObtenerVentasSemanales()
        {
            var hoy = DateTime.Today;
            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);
            var finSemana = inicioSemana.AddDays(6);

            var result = new List<DatosGraficoVM>();

            // Obtener ventas de los últimos 7 días
            for (int i = 0; i < 7; i++)
            {
                var dia = inicioSemana.AddDays(i);
                var facturasDia = _context.Facturas
                    .Where(f => f.FechaPago.Date == dia)
                    .ToList();

                result.Add(new DatosGraficoVM
                {
                    Etiqueta = dia.ToString("dd/MM"),
                    CantidadVentas = facturasDia.Count,
                    Ingresos = facturasDia.Sum(f => f.MontoTotal)
                });
            }

            return result;
        }

        private List<DatosGraficoVM> ObtenerVentasMensuales()
        {
            var hoy = DateTime.Today;
            var result = new List<DatosGraficoVM>();

            // Obtener ventas de los últimos 12 meses
            for (int i = 11; i >= 0; i--)
            {
                var mes = hoy.AddMonths(-i);
                var facturasMes = _context.Facturas
                    .Where(f => f.FechaPago.Year == mes.Year && f.FechaPago.Month == mes.Month)
                    .ToList();

                result.Add(new DatosGraficoVM
                {
                    Etiqueta = mes.ToString("MMM/yyyy"),
                    CantidadVentas = facturasMes.Count,
                    Ingresos = facturasMes.Sum(f => f.MontoTotal)
                });
            }

            return result;
        }
    }
}