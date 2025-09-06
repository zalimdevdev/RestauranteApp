using System.Collections.Generic;

namespace RestauranteApp.Models
{
    public class FacturaViewModel
    {
        // Esta es la factura principal (maestro)
        public Factura Factura { get; set; }

        // Esta es la lista de productos (detalle)
        public List<DetallePedido> Detalles { get; set; }

        public FacturaViewModel()
        {
            // Inicializamos para que no den error al usarlos
            Factura = new Factura();
            Detalles = new List<DetallePedido>();
        }
    }
}