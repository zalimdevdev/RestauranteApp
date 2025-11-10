using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteApp.ViewModels
{
    public class FacturaCreateVM
    {
        public FacturaCreateVM()
        {
            Clientes = new List<SelectListItem>();
            Mesas = new List<SelectListItem>();
            Items = new List<SelectListItem>();
            ItemPrecios = new Dictionary<int, decimal>();
        }

        public int FacturaId { get; set; }
        public int? ClienteId { get; set; }
        public int? MesaId { get; set; }
        public string MetodoPago { get; set; }
        public DateTime? FechaPago { get; set; }

        // Listas para los combos
        public IEnumerable<SelectListItem>? Clientes { get; set; }
        public IEnumerable<SelectListItem>? Mesas { get; set; }
        public IEnumerable<SelectListItem>? Items { get; set; }

        // Diccionario de precios por Item
        public Dictionary<int, decimal>? ItemPrecios { get; set; }

        // Detalles que el usuario agregará en el formulario
        public List<FacturaDetalleVM> Detalles { get; set; } = new();
    }

    public class FacturaDetalleVM
    {
        public int? ItemId { get; set; }
        public int Cantidad { get; set; }
    }
}
