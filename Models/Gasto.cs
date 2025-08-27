using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestauranteApp.Models;

public class Gasto
{
    [Key]
    public int GastoId { get; set; }

    [Required]
    [StringLength(200)]
    public string Descripcion { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Monto { get; set; }

    public DateTime? FechaGasto { get; set; }

    [Required]
    [StringLength(50)]
    public string CategoriaGasto { get; set; } // "Salarios", "Alquiler", "Suministros"
}