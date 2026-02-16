using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

/// <summary>
/// Mesa del restaurante. El estado se deriva automáticamente de la orden activa.
/// </summary>
public class Mesa
{
    [Key]
    public int MesaId { get; set; }

    /// <summary>
    /// Número identificador de la mesa
    /// </summary>
    public int NumeroMesa { get; set; }

    /// <summary>
    /// Capacidad de personas
    /// </summary>
    public int Capacidad { get; set; }

    /// <summary>
    /// Ubicación física de la mesa
    /// </summary>
    [StringLength(50)]
    public string? Ubicacion { get; set; }

    /// <summary>
    /// Estado manual (para reservas) - se ignora si hay orden activa
    /// </summary>
    [Required]
    [StringLength(20)]
    public string EstadoReservacion { get; set; } = "DISPONIBLE"; // "DISPONIBLE", "RESERVADA"

    // ==================== PROPIEDADES DE NAVEGACIÓN ====================

    public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();
    
    /// <summary>
    /// Historial de órdenes de esta mesa
    /// </summary>
    public virtual ICollection<Orden> Ordenes { get; set; } = new List<Orden>();

    // ==================== PROPIEDADES DE CÁLCULO (No mapeadas a BD) ====================

    /// <summary>
    /// Orden activa de la mesa (null si no hay)
    /// </summary>
    [NotMapped]
    public Orden? OrdenActiva { get; set; }

    /// <summary>
    /// Estado efectivo de la mesa (deriva de orden activa)
    /// </summary>
    [NotMapped]
    public string EstadoEfectivo
    {
        get
        {
            if (OrdenActiva != null && OrdenActiva.EstaActiva)
            {
                return "OCUPADA";
            }
            return EstadoReservacion;
        }
    }

    /// <summary>
    /// Estado para compatibilidad con vistas antiguas
    /// </summary>
    [NotMapped]
    public string Estado => EstadoEfectivo;

    /// <summary>
    /// Indica si la mesa está disponible para nueva orden
    /// </summary>
    [NotMapped]
    public bool EstaDisponible => string.Equals(EstadoEfectivo, "DISPONIBLE", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Indica si la mesa tiene orden activa
    /// </summary>
    [NotMapped]
    public bool TieneOrdenActiva => OrdenActiva != null && OrdenActiva.EstaActiva;
}