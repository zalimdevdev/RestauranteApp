using System.ComponentModel.DataAnnotations;

namespace RestauranteApp.Models;

public class BackupRegistro
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string NombreArchivo { get; set; }

    [Required]
    [StringLength(500)]
    public string RutaArchivo { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public long TamanoBytes { get; set; }

    [StringLength(50)]
    public string TipoBackup { get; set; } // "Completo" o "Parcial"

    [StringLength(20)]
    public string Estado { get; set; } = "Completado"; // "Completado", "Fallido", "Restaurando"

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [StringLength(100)]
    public string? CreadoPor { get; set; }
}