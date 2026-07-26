using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models
{
    /// <summary>
    /// Registra los movimientos de entrada y salida de stock de ingredientes.
    /// </summary>
    public class MovimientoStock
    {
        [Key]
        public int MovimientoId { get; set; }

        [Required]
        public int IngredienteId { get; set; }

        /// <summary>
        /// Tipo de movimiento: Entrada (reabastecimiento) o Salida (venta/uso)
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Tipo { get; set; } = string.Empty; // "Entrada" o "Salida"

        /// <summary>
        /// Cantidad del movimiento
        /// </summary>
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Fecha y hora del movimiento
        /// </summary>
        public DateTime Fecha { get; set; } = DateTime.Now;

        /// <summary>
        /// ID de la factura que generó el movimiento (para salidas)
        /// </summary>
        public int? FacturaId { get; set; }

        /// <summary>
        /// ID del detalle de factura que generó el movimiento (para salidas)
        /// </summary>
        public int? DetalleFacturaId { get; set; }

        /// <summary>
        /// Observaciones del movimiento
        /// </summary>
        [StringLength(250)]
        public string? Observacion { get; set; }

        // Propiedades de navegación
        [ForeignKey(nameof(IngredienteId))]
        public virtual Ingrediente? Ingrediente { get; set; }

        [ForeignKey(nameof(FacturaId))]
        public virtual Factura? Factura { get; set; }

        [ForeignKey(nameof(DetalleFacturaId))]
        public virtual DetalleFactura? DetalleFactura { get; set; }
    }
}