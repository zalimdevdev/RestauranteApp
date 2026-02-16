using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestauranteApp.Models;

/// <summary>
/// Factura generada desde una orden. La factura es única por orden.
/// </summary>
public class Factura
{
    [Key]
    public int FacturaId { get; set; }

    /// <summary>
    /// ID de la orden de la cual se genera esta factura
    /// </summary>
    public int? OrdenId { get; set; }

    public int? ClienteId { get; set; }

    /// <summary>
    /// MesaId para compatibilidad con código antiguo
    /// </summary>
    public int? MesaId { get; set; }

    /// <summary>
    /// Número de factura (secuencial)
    /// </summary>
    public int NumeroFactura { get; set; }

    /// <summary>
    /// Subtotal antes de impuestos
    /// </summary>
    [Precision(18, 2)]
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Monto del impuesto
    /// </summary>
    [Precision(18, 2)]
    public decimal MontoImpuesto { get; set; }

    /// <summary>
    /// Descuento aplicado
    /// </summary>
    [Precision(18, 2)]
    public decimal Descuento { get; set; }

    /// <summary>
    /// Total final de la factura
    /// </summary>
    [Precision(18, 2)]
    public decimal MontoTotal { get; set; }

    [StringLength(50)]
    public string MetodoPago { get; set; } = string.Empty;

    public DateTime FechaPago { get; set; } = DateTime.Now;

    /// <summary>
    /// Estado de la factura (PAGADA, PENDIENTE, ANULADA)
    /// </summary>
    [StringLength(20)]
    public string Estado { get; set; } = "PENDIENTE";

    // ==================== PROPIEDADES DE NAVEGACIÓN ====================

    /// <summary>
    /// Orden asociada (nueva relación)
    /// </summary>
    [ForeignKey(nameof(OrdenId))]
    public virtual Orden? Orden { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public virtual Cliente? Cliente { get; set; }

    [ForeignKey(nameof(MesaId))]
    public virtual Mesa? Mesa { get; set; }

    /// <summary>
    /// Detalles copiados de la orden al momento de facturar
    /// </summary>
    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();
}
