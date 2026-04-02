using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteApp.Services;

namespace RestauranteApp.Controllers;

[Authorize]
public class BackupsController : Controller
{
    private readonly BackupService _backupService;
    private readonly ILogger<BackupsController> _logger;

    public BackupsController(BackupService backupService, ILogger<BackupsController> logger)
    {
        _backupService = backupService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var backups = await _backupService.ObtenerTodosLosBackupsAsync();
        return View(backups);
    }

    [HttpPost]
    public async Task<IActionResult> CrearBackupCompleto(string? descripcion)
    {
        try
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var backup = await _backupService.CrearBackupCompletoAsync(descripcion, usuario);
            
            TempData["Success"] = $"Backup completo creado exitosamente: {backup.NombreArchivo}";
            _logger.LogInformation("Backup completo creado por {Usuario}: {FileName}", usuario, backup.NombreArchivo);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al crear backup: {ex.Message}";
            _logger.LogError(ex, "Error al crear backup completo");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CrearBackupParcial(string tipo, string? descripcion)
    {
        try
        {
            var usuario = User.Identity?.Name ?? "Sistema";
            var backup = await _backupService.CrearBackupParcialAsync(tipo, descripcion, usuario);
            
            TempData["Success"] = $"Backup de {tipo} creado exitosamente: {backup.NombreArchivo}";
            _logger.LogInformation("Backup parcial {Tipo} creado por {Usuario}: {FileName}", tipo, usuario, backup.NombreArchivo);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al crear backup: {ex.Message}";
            _logger.LogError(ex, "Error al crear backup parcial de tipo {Tipo}", tipo);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> RestaurarBackup(int id)
    {
        try
        {
            var resultado = await _backupService.RestaurarBackupAsync(id);
            
            if (resultado)
            {
                _logger.LogInformation("Backup {Id} restaurado por {Usuario}", id, User.Identity?.Name);
                return Json(new { success = true, message = "Backup restaurado exitosamente" });
            }
            else
            {
                return Json(new { success = false, message = "No se pudo restaurar el backup" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restaurar backup {Id}", id);
            return Json(new { success = false, message = "Error al restaurar: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> EliminarBackup(int id)
    {
        try
        {
            var resultado = await _backupService.EliminarBackupAsync(id);
            
            if (resultado)
            {
                TempData["Success"] = "Backup eliminado exitosamente";
                _logger.LogInformation("Backup {Id} eliminado por {Usuario}", id, User.Identity?.Name);
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el backup";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al eliminar backup: {ex.Message}";
            _logger.LogError(ex, "Error al eliminar backup {Id}", id);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Descargar(int id)
    {
        try
        {
            var bytes = await _backupService.DescargarBackupAsync(id);
            if (bytes == null)
            {
                TempData["Error"] = "Backup no encontrado";
                return RedirectToAction(nameof(Index));
            }

            var backup = await _backupService.ObtenerBackupPorIdAsync(id);
            return File(bytes, "application/json", backup?.NombreArchivo ?? "backup.json");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al descargar backup: {ex.Message}";
            _logger.LogError(ex, "Error al descargar backup {Id}", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50MB limit
    public async Task<IActionResult> ImportarBackup(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            TempData["Error"] = "Por favor seleccione un archivo";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var fileName = $"importado_{DateTime.Now:yyyyMMdd_HHmmss}_{archivo.FileName}";
            var backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
            
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            var filePath = Path.Combine(backupFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var fileInfo = new FileInfo(filePath);
            
            // Registrar el backup importado
            var registro = new Models.BackupRegistro
            {
                NombreArchivo = fileName,
                RutaArchivo = $"/backups/{fileName}",
                FechaCreacion = DateTime.Now,
                TamanoBytes = fileInfo.Length,
                TipoBackup = "Importado",
                Estado = "Completado",
                Descripcion = $"Importado manualmente",
                CreadoPor = User.Identity?.Name ?? "Sistema"
            };

            // Usar el contexto directamente para guardar el registro
            var context = HttpContext.RequestServices.GetService<Models.AppDbContext>();
            if (context != null)
            {
                context.BackupRegistros.Add(registro);
                await context.SaveChangesAsync();
            }

            TempData["Success"] = "Backup importado exitosamente";
            _logger.LogInformation("Backup importado por {Usuario}: {FileName}", User.Identity?.Name, fileName);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al importar backup: {ex.Message}";
            _logger.LogError(ex, "Error al importar backup");
        }

        return RedirectToAction(nameof(Index));
    }
}