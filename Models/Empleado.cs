using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class Empleado
{
    [Key]
    public int EmpleadoId { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombre { get; set; }

    [Required]
    [StringLength(50)]
    public string Apellido { get; set; }

    [Required]
    [StringLength(50)]
    public string Puesto { get; set; }

    public DateTime FechaContratacion { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Salario { get; set; }
    
    [StringLength(15)]
    public string? Telefono { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}