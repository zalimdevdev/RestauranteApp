using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class ItemMenuIngrediente
{
    [Key]
    public int Id { get; set; }

    public int ItemMenuId { get; set; }
    public int IngredienteId { get; set; }
    [Column(TypeName = "decimal(18, 3)")]
    public decimal Cantidad { get; set; }

    public virtual ItemMenu? ItemMenu { get; set; }
    public virtual Ingrediente? Ingrediente { get; set; }
}