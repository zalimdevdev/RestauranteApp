using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class Cliente
{
    [Key]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50)]
    public string Apellido { get; set; }

    [StringLength(15)]
    public string? Telefono { get; set; }

    [StringLength(15)]
    public string? Cedula { get; set; }


    [StringLength(15)]
    public string? Direccion { get; set; }    


    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // Propiedades de navegación
    public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();
    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}