using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class DetallePedido
{
    [Key]
    public int DetalleId { get; set; }

    public int PedidoId { get; set; }
    public int ItemId { get; set; }

    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioUnitario { get; set; } // Se guarda para mantener el precio histórico

    // Propiedades de navegación
    [ForeignKey("PedidoId")]
    public virtual Pedido? Pedido { get; set; }

    [ForeignKey("ItemId")]
    public virtual ItemMenu? ItemMenu { get; set; }
}