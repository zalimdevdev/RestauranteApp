using System.ComponentModel.DataAnnotations;

namespace RestauranteApp.Models;

public class Proveedor
{
    [Key]
    public int ProveedorId { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreProveedor { get; set; }

    [StringLength(100)]
    public string? Contacto { get; set; }

    [StringLength(15)]
    public string? Telefono { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }
    
    // Propiedad de navegación
}