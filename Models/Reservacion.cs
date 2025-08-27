using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class Reservacion
{
    [Key]
    public int ReservacionId { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required]
    public int MesaId { get; set; }

    public DateTime FechaHora { get; set; }

    public int NumeroPersonas { get; set; }

    [Required]
    [StringLength(20)]
    public string Estado { get; set; } // "Pendiente", "Confirmada", "Cancelada"

    // Propiedades de navegación
    [ForeignKey("ClienteId")]
    public virtual Cliente? Cliente { get; set; }
    
    [ForeignKey("MesaId")]
    public virtual Mesa? Mesa { get; set; }
}