using System.ComponentModel.DataAnnotations;

namespace RestauranteApp.Models;

public class CategoriaMenu
{
    [Key]
    public int CategoriaId { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreCategoria { get; set; }

    // Propiedad de navegación
    public virtual ICollection<ItemMenu> ItemsMenu { get; set; } = new List<ItemMenu>();
}