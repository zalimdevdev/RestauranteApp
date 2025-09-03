using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class Factura
{
    [Key]
    public int FacturaId { get; set; }

    [Required]
    public int PedidoId { get; set; }

    public int? ClienteId { get; set; } // Nullable, ya que una venta puede ser a un cliente no registrado

    public int? MesaId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal MontoTotal { get; set; }

    [StringLength(50)]
    public string MetodoPago { get; set; }


//public string Estado { get; set; }


    public DateTime FechaPago { get; set; } = DateTime.Now;

    // Propiedades de navegación


//     [ForeignKey("ItemMenuId")]
//     public virtual ItemMenu? ItemMenu { get; set; }


    // Propiedades de navegación
    [ForeignKey("PedidoId")]
    public virtual Pedido? Pedido { get; set; }

    [ForeignKey("ClienteId")]
    public virtual Cliente? Cliente { get; set; }

    [ForeignKey("MesaId")]
    public virtual Mesa? Mesa { get; set; }

}