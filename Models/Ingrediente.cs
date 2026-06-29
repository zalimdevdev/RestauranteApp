using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class Ingrediente
{
    [Key]
    public int IngredienteId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string NombreIngrediente { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal CantidadStock { get; set; }

    [Required]
    [StringLength(20)]
    public string UnidadMedida { get; set; } // "kg", "litros", "unidades"

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Costo { get; set; } // Costo por unidad de medida

    [Required]
    [StringLength(50)]
    public string Categoria { get; set; } // Categoría del ingrediente (ej: Verdura, Carne, etc.)
}