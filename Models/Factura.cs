using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestauranteApp.Models
{
    public class Factura
    {
        [Key]
        public int FacturaId { get; set; }

        public int? ClienteId { get; set; } // Nullable, venta a cliente no registrado
        public int? MesaId { get; set; }

        [Precision(18, 2)]
        public decimal MontoTotal { get; set; }
        [StringLength(50)]
        public string MetodoPago { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; } = DateTime.Now;

        // Colección de detalles (relación 1 - N)
        public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

        // Navegación a otras entidades
        [ForeignKey(nameof(ClienteId))]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey(nameof(MesaId))]
        public virtual Mesa? Mesa { get; set; }
    }
}
