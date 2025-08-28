using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class DatosNegocio
{

    [Key]
    public int DatosNegocioId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; }

    [StringLength(15)]
    public string? Telefono { get; set; }

    [StringLength(15)]
    public string? Ruc { get; set; }

    [StringLength(15)]
    public string? DireccionNegocio { get; set; } 


}