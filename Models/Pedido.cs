using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class Pedido
{
    [Key]
    public int PedidoId { get; set; }

    public int MesaId { get; set; }
    public int EmpleadoId { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.Now;

    [Required]
    [StringLength(25)]
    public string EstadoPedido { get; set; } // "En preparación", "Servido", "Pagado"

    // Propiedades de navegación
    [ForeignKey("MesaId")]
    public virtual Mesa? Mesa { get; set; }

    [ForeignKey("EmpleadoId")]
    public virtual Empleado? Empleado { get; set; }

    public virtual ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
    public virtual Factura? Factura { get; set; }
}