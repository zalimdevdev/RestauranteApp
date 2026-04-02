using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteApp.ViewModels
{
    public class MesaVentaVM
    {
        public int MesaId { get; set; }
        public int NumeroMesa { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; } = "Disponible";
        
        // Información de la factura activa (si la mesa está ocupada)
        public int? FacturaId { get; set; }
        public decimal? MontoTotal { get; set; }
        public string? MetodoPago { get; set; }
        public DateTime? FechaPago { get; set; }
        
        // Indica si la mesa tiene una venta activa
        public bool TieneVentaActiva => FacturaId.HasValue;
    }

    public class GestionMesasVM
    {
        public List<MesaVentaVM> Mesas { get; set; } = new();
        public int? MesaSeleccionadaId { get; set; }
    }
}