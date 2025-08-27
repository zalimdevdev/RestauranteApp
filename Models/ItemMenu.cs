using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class ItemMenu
{
    [Key]
    public int ItemId { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreItem { get; set; }

    public string? Descripcion { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Precio { get; set; }

    public int CategoriaId { get; set; }

    // Propiedades de navegación
    [ForeignKey("CategoriaId")]
    public virtual CategoriaMenu? CategoriaMenu { get; set; }
    public virtual ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
}