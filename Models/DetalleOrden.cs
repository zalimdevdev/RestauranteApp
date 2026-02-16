using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

/// <summary>
/// Detalle de una orden - items agregados a la orden
/// </summary>
public class DetalleOrden
{
    [Key]
    public int DetalleOrdenId { get; set; }

    [Required]
    public int OrdenId { get; set; }

    [Required]
    public int ItemMenuId { get; set; }

    [Required]
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario al momento de agregar (para mantener historial)
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Notas especiales para este item (sin cebolla, extra queso, etc.)
    /// </summary>
    [StringLength(200)]
    public string? Notas { get; set; }

    /// <summary>
    /// Estado del detalle (Pendiente, Preparando, Servido)
    /// </summary>
    public bool EstaServido { get; set; }

    /// <summary>
    /// Fecha/hora en que se marcó como servido
    /// </summary>
    public DateTime? FechaHoraServido { get; set; }

    /// <summary>
    /// Subtotal de este detalle (Cantidad * PrecioUnitario)
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Subtotal => Cantidad * PrecioUnitario;

    // ==================== PROPIEDADES DE NAVEGACIÓN ====================

    [ForeignKey("OrdenId")]
    public virtual Orden? Orden { get; set; }

    [ForeignKey("ItemMenuId")]
    public virtual ItemMenu? ItemMenu { get; set; }
}