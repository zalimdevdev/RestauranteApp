using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestauranteApp.Models
{
    public class DetalleFactura
    {
        [Key]
        public int DetalleFacturaId { get; set; }

        [Required]
        public int FacturaId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        // Usa Precision para compatibilidad con EF Core + PostgreSQL
        [Precision(18, 2)]
        public decimal PrecioUnitario { get; set; }

        [Precision(18, 2)]
        public decimal Subtotal { get; set; }

        // Navegaciones
        [ForeignKey(nameof(FacturaId))]
        public virtual Factura Factura { get; set; } = null!;

        [ForeignKey(nameof(ItemId))]
        public virtual ItemMenu ItemMenu { get; set; } = null!;
    }
}
