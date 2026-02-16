using System.ComponentModel.DataAnnotations;

namespace RestauranteApp.Models;

public class CategoriaMenu
{
    [Key]
    public int CategoriaId { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreCategoria { get; set; }

    [StringLength(70)]
    public string DescripcionCategoria { get; set; }

    /// <summary>
    /// Estado de la categoría (para compatibilidad)
    /// </summary>
    [StringLength(20)]
    public string Estado { get; set; } = "ACTIVO";

    // Propiedad de navegación
    public virtual ICollection<ItemMenu> ItemsMenu { get; set; } = new List<ItemMenu>();
}