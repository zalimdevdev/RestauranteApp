using System.ComponentModel.DataAnnotations;

namespace RestauranteApp.Models;

public class Mesa
{
    [Key]
    public int MesaId { get; set; }

    public int NumeroMesa { get; set; }

    public int Capacidad { get; set; }

    [Required]
    [StringLength(20)]
    public string Estado { get; set; } // "Disponible", "Ocupada", "Reservada"

    // Propiedades de navegación
    public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}