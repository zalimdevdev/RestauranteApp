using System.Collections.Generic;

namespace RestauranteApp.ViewModels
{
    public class EstadisticasVM
    {
        public int MesSeleccionado { get; set; }
        public int AnioSeleccionado { get; set; }

        public IEnumerable<ProductoVentaVM> ProductosMasVendidos { get; set; } = new List<ProductoVentaVM>();
        public IEnumerable<ProductoVentaVM> ProductosMenosVendidos { get; set; } = new List<ProductoVentaVM>();

        // Datos para gráficos
        public List<DatosGraficoVM> VentasDiarias { get; set; } = new List<DatosGraficoVM>();
        public List<DatosGraficoVM> VentasSemanales { get; set; } = new List<DatosGraficoVM>();
        public List<DatosGraficoVM> VentasMensuales { get; set; } = new List<DatosGraficoVM>();
    }

    public class ProductoVentaVM
    {
        public int ItemId { get; set; }
        public string NombreItem { get; set; } = string.Empty;
        public int TotalVendido { get; set; }
        public decimal TotalIngresos { get; set; }
    }

    public class DatosGraficoVM
    {
        public string Etiqueta { get; set; } = string.Empty;
        public int CantidadVentas { get; set; }
        public decimal Ingresos { get; set; }
    }
}