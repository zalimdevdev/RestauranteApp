using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;
using RestauranteApp.Models.Enums;
using RestauranteApp.Services;
using RestauranteApp.ViewModels;

namespace RestauranteApp.Controllers;

public class TableroPosController : Controller
{
    private readonly AppDbContext _context;
    private readonly OrdenService _ordenService;

    public TableroPosController(AppDbContext context, OrdenService ordenService)
    {
        _context = context;
        _ordenService = ordenService;
    }

    // ==================== VISTAS PRINCIPALES ====================

    /// <summary>
    /// Muestra el tablero de mesas del POS
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var modelo = new TableroPosVM();
        
        try
        {
            var mesas = await _ordenService.ObtenerTableroMesas();
            
            foreach (var mesa in mesas)
            {
                var mesaVM = new MesaVM
                {
                    MesaId = mesa.MesaId,
                    NumeroMesa = mesa.NumeroMesa,
                    Capacidad = mesa.Capacidad,
                    Ubicacion = mesa.Ubicacion,
                    Estado = mesa.EstadoEfectivo,
                    TieneOrdenActiva = mesa.TieneOrdenActiva
                };

                if (mesa.OrdenActiva != null)
                {
                    mesaVM.OrdenActiva = MapearOrdenVM(mesa.OrdenActiva);
                }

                modelo.Mesas.Add(mesaVM);
            }

            modelo.TotalMesas = modelo.Mesas.Count;
            modelo.MesasDisponibles = modelo.Mesas.Count(m => m.Estado == "DISPONIBLE");
            modelo.MesasOcupadas = modelo.Mesas.Count(m => m.Estado == "OCUPADA");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al cargar el tablero: {ex.Message}";
        }

        return View(modelo);
    }

    /// <summary>
    /// Muestra los detalles de una orden
    /// </summary>
    public async Task<IActionResult> DetallesOrden(int id)
    {
        var orden = await _ordenService.ObtenerOrdenPorId(id);
        if (orden == null)
        {
            return NotFound();
        }

        var modelo = MapearOrdenVM(orden);
        return View(modelo);
    }

    // ==================== GESTIÓN DE ÓRDENES ====================

    /// <summary>
    /// Abre una orden para una mesa (o la recupera si ya existe)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AbrirOrden(int mesaId)
    {
        try
        {
            // Obtener el empleado actual (en implementación real, obtener del contexto de autenticación)
            int? empleadoId = null;
            
            var orden = await _ordenService.AbrirOrdenMesa(mesaId, empleadoId);
            TempData["Success"] = $"Orden #{orden.NumeroOrden} abierta para la mesa {mesaId}";
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    /// Cancela una orden
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CancelarOrden(int ordenId, string motivo)
    {
        try
        {
            await _ordenService.CancelarOrden(ordenId, motivo);
            TempData["Success"] = "Orden cancelada correctamente";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    // ==================== GESTIÓN DE ITEMS ====================

    /// <summary>
    /// Muestra el modal para agregar items a una orden
    /// </summary>
    public async Task<IActionResult> AgregarItems(int ordenId)
    {
        var orden = await _ordenService.ObtenerOrdenPorId(ordenId);
        if (orden == null)
        {
            return NotFound();
        }

        ViewBag.Orden = orden;
        
        var categorias = await _context.CategoriasMenu
            .Include(c => c.ItemsMenu)
            .ToListAsync();
            
        var items = await _context.ItemsMenu
            .Where(i => i.Estado == "DISPONIBLE")
            .ToListAsync();

        var modelo = new MenuPosVM
        {
            Categorias = categorias,
            Items = items
        };

        return View(modelo);
    }

    /// <summary>
    /// Agrega un item a la orden
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AgregarItem(int ordenId, int itemMenuId, int cantidad, string? notas)
    {
        try
        {
            await _ordenService.AgregarItem(ordenId, itemMenuId, cantidad, notas);
            return Json(new { success = true, message = "Item agregado" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un item de la orden
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> EliminarItem(int detalleId)
    {
        try
        {
            await _ordenService.EliminarItem(detalleId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza la cantidad de un item
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ActualizarCantidad(int detalleId, int cantidad)
    {
        try
        {
            await _ordenService.ActualizarCantidad(detalleId, cantidad);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ==================== TRANSICIONES DE ESTADO ====================

    /// <summary>
    /// Envía la orden a preparación
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> EnviarAPreparacion(int ordenId)
    {
        try
        {
            await _ordenService.CambiarEstado(ordenId, EstadoOrden.EN_PREPARACION);
            TempData["Success"] = "Orden enviada a cocina";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Marca la orden como servida
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> MarcarServida(int ordenId)
    {
        try
        {
            await _ordenService.CambiarEstado(ordenId, EstadoOrden.SERVIDA);
            TempData["Success"] = "Orden marcada como servida";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Cliente solicita la cuenta
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SolicitarCuenta(int ordenId)
    {
        try
        {
            await _ordenService.CambiarEstado(ordenId, EstadoOrden.SOLICITA_CUENTA);
            TempData["Success"] = "Cuenta solicitada";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    // ==================== FACTURACIÓN ====================

    /// <summary>
    /// Muestra el formulario de facturación
    /// </summary>
    public async Task<IActionResult> Facturar(int ordenId)
    {
        var orden = await _ordenService.ObtenerOrdenPorId(ordenId);
        if (orden == null)
        {
            return NotFound();
        }

        ViewBag.Orden = orden;
        
        var clientes = await _context.Clientes.ToListAsync();
        ViewBag.Clientes = clientes;

        var modelo = new FacturarVM
        {
            OrdenId = ordenId
        };

        return View(modelo);
    }

    /// <summary>
    /// Genera la factura y cambia el estado a FACTURADA
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Facturar(FacturarVM modelo)
    {
        try
        {
            var factura = await _ordenService.FacturarOrden(
                modelo.OrdenId, 
                modelo.ClienteId, 
                modelo.Descuento);

            TempData["Success"] = $"Factura #{factura.NumeroFactura} generada correctamente";
            
            // Redireccionar al método de pago
            return RedirectToAction("RegistrarPago", new { ordenId = modelo.OrdenId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    /// Muestra el formulario de pago
    /// </summary>
    public async Task<IActionResult> RegistrarPago(int ordenId)
    {
        var orden = await _ordenService.ObtenerOrdenPorId(ordenId);
        if (orden == null || orden.Factura == null)
        {
            return NotFound();
        }

        ViewBag.Orden = orden;
        ViewBag.Factura = orden.Factura;

        return View();
    }

    /// <summary>
    /// Procesa el pago y cierra la orden
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProcesarPago(int ordenId, string metodoPago)
    {
        try
        {
            await _ordenService.RegistrarPagoYCerrar(ordenId, metodoPago);
            TempData["Success"] = "Pago registrado. Orden cerrada correctamente";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    // ==================== MÉTODOS AUXILIARES ====================

    private OrdenVM MapearOrdenVM(Orden orden)
    {
        var ordenVM = new OrdenVM
        {
            OrdenId = orden.OrdenId,
            NumeroOrden = orden.NumeroOrden,
            Estado = orden.Estado.ToString(),
            NombreEstado = orden.NombreEstado,
            FechaHora = orden.FechaHoraCreacion,
            Total = orden.Total,
            CantidadItems = orden.Detalles.Sum(d => d.Cantidad),
            Detalles = orden.Detalles.Select(d => new DetalleOrdenVM
            {
                DetalleOrdenId = d.DetalleOrdenId,
                NombreItem = d.ItemMenu?.NombreItem ?? "",
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal,
                Notas = d.Notas,
                EstaServido = d.EstaServido
            }).ToList()
        };

        // Determinar botones de acción según el estado
        ordenVM.PuedeAgregarItems = orden.Estado == EstadoOrden.ABIERTA;
        ordenVM.PuedeEnviarACocina = orden.Estado == EstadoOrden.ABIERTA && orden.Detalles.Any();
        ordenVM.PuedeMarcarServida = orden.Estado == EstadoOrden.EN_PREPARACION;
        ordenVM.PuedeSolicitarCuenta = orden.Estado == EstadoOrden.SERVIDA;
        ordenVM.PuedeFacturar = orden.Estado == EstadoOrden.SOLICITA_CUENTA;
        ordenVM.PuedeCerrar = orden.Estado == EstadoOrden.FACTURADA;
        ordenVM.PuedeCancelar = orden.Estado != EstadoOrden.CERRADA && orden.Estado != EstadoOrden.CANCELADA;

        return ordenVM;
    }
}