using System.Collections.Generic;

namespace RestauranteApp.ViewModels
{
    public class EstadisticasVM
    {
        public int MesSeleccionado { get; set; }
        public int AnioSeleccionado { get; set; }

        public IEnumerable<ProductoVentaVM> ProductosMasVendidos { get; set; } = new List<ProductoVentaVM>();
        public IEnumerable<ProductoVentaVM> ProductosMenosVendidos { get; set; } = new List<ProductoVentaVM>();
    }

    public class ProductoVentaVM
    {
        public int ItemId { get; set; }
        public string NombreItem { get; set; } = string.Empty;
        public int TotalVendido { get; set; }
        public decimal TotalIngresos { get; set; }
    }
}