using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestauranteApp.Models.Enums;

namespace RestauranteApp.Models;

/// <summary>
/// Entidad central del sistema POS. Representa una orden/pedido en una mesa.
/// </summary>
public class Orden
{
    [Key]
    public int OrdenId { get; set; }

    [Required]
    public int MesaId { get; set; }

    public int? EmpleadoId { get; set; }

    public int? ClienteId { get; set; }

    /// <summary>
    /// Fecha y hora de creación de la orden
    /// </summary>
    public DateTime FechaHoraCreacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha y hora de última modificación
    /// </summary>
    public DateTime FechaHoraModificacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Estado actual de la orden
    /// </summary>
    public EstadoOrden Estado { get; set; } = EstadoOrden.ABIERTA;

    /// <summary>
    /// Notas adicionales de la orden
    /// </summary>
    [StringLength(500)]
    public string? Notas { get; set; }

    /// <summary>
    /// Subtotal antes de impuestos
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Monto del impuesto (ej. IVA)
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoImpuesto { get; set; }

    /// <summary>
    /// Descuento aplicado a la orden
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Descuento { get; set; }

    /// <summary>
    /// Total final de la orden
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Total { get; set; }

    /// <summary>
    /// Número único de orden para identificación
    /// </summary>
    public int NumeroOrden { get; set; }

    // ==================== PROPIEDADES DE NAVEGACIÓN ====================

    [ForeignKey("MesaId")]
    public virtual Mesa? Mesa { get; set; }

    [ForeignKey("EmpleadoId")]
    public virtual Empleado? Empleado { get; set; }

    [ForeignKey("ClienteId")]
    public virtual Cliente? Cliente { get; set; }

    /// <summary>
    /// Detalles/items de la orden
    /// </summary>
    public virtual ICollection<DetalleOrden> Detalles { get; set; } = new List<DetalleOrden>();

    /// <summary>
    /// Factura asociada a esta orden (nullable - se crea al facturar)
    /// </summary>
    public virtual Factura? Factura { get; set; }

    // ==================== PROPIEDADES DE CÁLCULO ====================

    /// <summary>
    /// Indica si la orden tiene una factura asociada
    /// </summary>
    [NotMapped]
    public bool TieneFactura => Factura != null;

    /// <summary>
    /// Indica si la orden está activa (no cerrada ni cancelada)
    /// </summary>
    [NotMapped]
    public bool EstaActiva => Estado != EstadoOrden.CERRADA && Estado != EstadoOrden.CANCELADA;

    /// <summary>
    /// Indica si la orden puede ser modificada
    /// </summary>
    [NotMapped]
    public bool PuedeModificarse => Estado == EstadoOrden.ABIERTA || Estado == EstadoOrden.EN_PREPARACION;

    /// <summary>
    /// Nombre del estado para display
    /// </summary>
    [NotMapped]
    public string NombreEstado => Estado.ToString().Replace("_", " ");

    /// <summary>
    /// Cantidad total de items en la orden
    /// </summary>
    [NotMapped]
    public int CantidadItems => Detalles?.Sum(d => d.Cantidad) ?? 0;
}