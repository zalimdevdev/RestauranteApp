using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;
using RestauranteApp.Models.Enums;

namespace RestauranteApp.Services;

/// <summary>
/// Servicio de negocio para gestionar órdenes del POS
/// </summary>
public class OrdenService
{
    private readonly AppDbContext _context;

    public OrdenService(AppDbContext context)
    {
        _context = context;
    }

    // ==================== CONSULTA DE DATOS ====================

    /// <summary>
    /// Obtiene el tablero de todas las mesas con su estado efectivo
    /// </summary>
    public async Task<List<Mesa>> ObtenerTableroMesas()
    {
        // Obtener todas las órdenes activas
        var ordenesActivas = await _context.Ordenes
            .Where(o => o.Estado != EstadoOrden.CERRADA && o.Estado != EstadoOrden.CANCELADA)
            .ToListAsync();

        // Obtener todas las mesas
        var mesas = await _context.Mesas.ToListAsync();

        // Asignar la orden activa a cada mesa
        foreach (var mesa in mesas)
        {
            mesa.OrdenActiva = ordenesActivas.FirstOrDefault(o => o.MesaId == mesa.MesaId);
        }

        return mesas;
    }

    /// <summary>
    /// Obtiene una orden por su ID con todos sus detalles
    /// </summary>
    public async Task<Orden?> ObtenerOrdenPorId(int ordenId)
    {
        return await _context.Ordenes
            .Include(o => o.Detalles)
                .ThenInclude(d => d.ItemMenu)
            .Include(o => o.Mesa)
            .Include(o => o.Empleado)
            .Include(o => o.Factura)
            .FirstOrDefaultAsync(o => o.OrdenId == ordenId);
    }

    /// <summary>
    /// Obtiene la orden activa de una mesa específica
    /// </summary>
    public async Task<Orden?> ObtenerOrdenActivaPorMesa(int mesaId)
    {
        return await _context.Ordenes
            .Include(o => o.Detalles)
                .ThenInclude(d => d.ItemMenu)
            .Include(o => o.Factura)
            .FirstOrDefaultAsync(o => 
                o.MesaId == mesaId && 
                o.Estado != EstadoOrden.CERRADA && 
                o.Estado != EstadoOrden.CANCELADA);
    }

    // ==================== CREACIÓN DE ÓRDENES ====================

    /// <summary>
    /// Crea una nueva orden para una mesa. Si ya existe una orden activa, la retorna.
    /// </summary>
    public async Task<Orden> CrearOrden(int mesaId, int? empleadoId, int? clienteId = null)
    {
        // Verificar si ya existe una orden activa para esta mesa
        var ordenActiva = await ObtenerOrdenActivaPorMesa(mesaId);
        if (ordenActiva != null)
        {
            return ordenActiva;
        }

        // Verificar que la mesa existe
        var mesa = await _context.Mesas.FindAsync(mesaId);
        if (mesa == null)
        {
            throw new InvalidOperationException($"La mesa {mesaId} no existe");
        }

        // Generar número de orden único
        var ultimoNumero = await _context.Ordenes.MaxAsync(o => (int?)o.NumeroOrden) ?? 0;

        // Crear la nueva orden
        var orden = new Orden
        {
            MesaId = mesaId,
            EmpleadoId = empleadoId,
            ClienteId = clienteId,
            FechaHoraCreacion = DateTime.Now,
            FechaHoraModificacion = DateTime.Now,
            Estado = EstadoOrden.ABIERTA,
            NumeroOrden = ultimoNumero + 1,
            Subtotal = 0,
            MontoImpuesto = 0,
            Descuento = 0,
            Total = 0
        };

        _context.Ordenes.Add(orden);
        await _context.SaveChangesAsync();

        return orden;
    }

    /// <summary>
    /// Abre o retrieve una orden existente para una mesa
    /// </summary>
    public async Task<Orden> AbrirOrdenMesa(int mesaId, int? empleadoId)
    {
        return await CrearOrden(mesaId, empleadoId);
    }

    // ==================== GESTIÓN DE ITEMS ====================

    /// <summary>
    /// Agrega un item a la orden
    /// </summary>
    public async Task<DetalleOrden> AgregarItem(int ordenId, int itemMenuId, int cantidad, string? notas = null)
    {
        var orden = await ObtenerOrdenPorId(ordenId);
        if (orden == null)
        {
            throw new InvalidOperationException($"La orden {ordenId} no existe");
        }

        if (!orden.PuedeModificarse)
        {
            throw new InvalidOperationException($"No se puede modificar una orden en estado {orden.NombreEstado}");
        }

        var itemMenu = await _context.ItemsMenu.FindAsync(itemMenuId);
        if (itemMenu == null)
        {
            throw new InvalidOperationException($"El item {itemMenuId} no existe");
        }

        // Verificar si el item ya está en la orden (misma configuración)
        var detalleExistente = orden.Detalles.FirstOrDefault(d => 
            d.ItemMenuId == itemMenuId && 
            d.Notas == notas && 
            !d.EstaServido);

        if (detalleExistente != null)
        {
            // Aumentar cantidad
            detalleExistente.Cantidad += cantidad;
            _context.DetallesOrden.Update(detalleExistente);
        }
        else
        {
            // Crear nuevo detalle
            var detalle = new DetalleOrden
            {
                OrdenId = ordenId,
                ItemMenuId = itemMenuId,
                Cantidad = cantidad,
                PrecioUnitario = itemMenu.Precio,
                Notas = notas,
                EstaServido = false
            };
            _context.DetallesOrden.Add(detalle);
        }

        // Recalcular totales
        await RecalcularTotales(ordenId);
        
        // Actualizar fecha de modificación
        orden.FechaHoraModificacion = DateTime.Now;
        await _context.SaveChangesAsync();

        return orden.Detalles.First(d => d.ItemMenuId == itemMenuId && d.OrdenId == ordenId);
    }

    /// <summary>
    /// Elimina un item de la orden
    /// </summary>
    public async Task EliminarItem(int detalleOrdenId)
    {
        var detalle = await _context.DetallesOrden
            .Include(d => d.Orden)
            .FirstOrDefaultAsync(d => d.DetalleOrdenId == detalleOrdenId);

        if (detalle == null)
        {
            throw new InvalidOperationException($"El detalle {detalleOrdenId} no existe");
        }

        if (detalle.Orden != null && !detalle.Orden.PuedeModificarse)
        {
            throw new InvalidOperationException("No se puede modificar una orden que no está en estado ABIERTA o EN_PREPARACION");
        }

        _context.DetallesOrden.Remove(detalle);
        
        // Recalcular totales
        await RecalcularTotales(detalle.OrdenId);
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza la cantidad de un item
    /// </summary>
    public async Task ActualizarCantidad(int detalleOrdenId, int nuevaCantidad)
    {
        if (nuevaCantidad <= 0)
        {
            await EliminarItem(detalleOrdenId);
            return;
        }

        var detalle = await _context.DetallesOrden
            .Include(d => d.Orden)
            .FirstOrDefaultAsync(d => d.DetalleOrdenId == detalleOrdenId);

        if (detalle == null)
        {
            throw new InvalidOperationException($"El detalle {detalleOrdenId} no existe");
        }

        if (detalle.Orden != null && !detalle.Orden.PuedeModificarse)
        {
            throw new InvalidOperationException("No se puede modificar una orden que no está en estado ABIERTA o EN_PREPARACION");
        }

        detalle.Cantidad = nuevaCantidad;
        
        // Recalcular totales
        await RecalcularTotales(detalle.OrdenId);
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Marca un item como servido
    /// </summary>
    public async Task MarcarItemServido(int detalleOrdenId)
    {
        var detalle = await _context.DetallesOrden.FindAsync(detalleOrdenId);
        if (detalle == null)
        {
            throw new InvalidOperationException($"El detalle {detalleOrdenId} no existe");
        }

        detalle.EstaServido = true;
        detalle.FechaHoraServido = DateTime.Now;
        
        await _context.SaveChangesAsync();
    }

    // ==================== TRANSICIONES DE ESTADO ====================

    /// <summary>
    /// Cambia el estado de una orden validando las transiciones permitidas
    /// </summary>
    public async Task<Orden> CambiarEstado(int ordenId, EstadoOrden nuevoEstado)
    {
        var orden = await ObtenerOrdenPorId(ordenId);
        if (orden == null)
        {
            throw new InvalidOperationException($"La orden {ordenId} no existe");
        }

        // Validar transición de estado
        if (!EsTransicionValida(orden.Estado, nuevoEstado))
        {
            throw new InvalidOperationException(
                $"Transición inválida: No se puede cambiar de {orden.NombreEstado} a {nuevoEstado.ToString().Replace("_", " ")}");
        }

        // Ejecutar lógica específica por transición
        switch (nuevoEstado)
        {
            case EstadoOrden.EN_PREPARACION:
                // Validar que hay items en la orden
                if (!orden.Detalles.Any())
                {
                    throw new InvalidOperationException("No se puede iniciar la preparación sin items en la orden");
                }
                break;

            case EstadoOrden.SOLICITA_CUENTA:
                // Validar que todos los items estén servidos
                if (orden.Detalles.Any(d => !d.EstaServido))
                {
                    throw new InvalidOperationException("No se puede solicitar la cuenta con items pendientes");
                }
                break;

            case EstadoOrden.CERRADA:
                // Validar que esté facturada
                if (orden.Factura == null)
                {
                    throw new InvalidOperationException("No se puede cerrar una orden sin facturar");
                }
                break;
        }

        orden.Estado = nuevoEstado;
        orden.FechaHoraModificacion = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        return orden;
    }

    /// <summary>
    /// Valida si una transición de estado es permitida
    /// </summary>
    private bool EsTransicionValida(EstadoOrden actual, EstadoOrden nuevo)
    {
        // Definir transiciones válidas
        return (actual, nuevo) switch
        {
            (EstadoOrden.ABIERTA, EstadoOrden.EN_PREPARACION) => true,
            (EstadoOrden.ABIERTA, EstadoOrden.CANCELADA) => true,
            (EstadoOrden.EN_PREPARACION, EstadoOrden.SERVIDA) => true,
            (EstadoOrden.EN_PREPARACION, EstadoOrden.CANCELADA) => true,
            (EstadoOrden.SERVIDA, EstadoOrden.SOLICITA_CUENTA) => true,
            (EstadoOrden.SOLICITA_CUENTA, EstadoOrden.FACTURADA) => true,
            (EstadoOrden.FACTURADA, EstadoOrden.CERRADA) => true,
            (EstadoOrden.FACTURADA, EstadoOrden.CANCELADA) => true,
            _ => false
        };
    }

    // ==================== FACTURACIÓN ====================

    /// <summary>
    /// Genera una factura a partir de la orden
    /// </summary>
    public async Task<Factura> FacturarOrden(int ordenId, int? clienteId = null, decimal descuento = 0)
    {
        var orden = await ObtenerOrdenPorId(ordenId);
        if (orden == null)
        {
            throw new InvalidOperationException($"La orden {ordenId} no existe");
        }

        // Validar que la orden esté en estado correcto
        if (orden.Estado != EstadoOrden.SOLICITA_CUENTA)
        {
            throw new InvalidOperationException($"La orden debe estar en estado SOLICITA_CUENTA para facturar. Estado actual: {orden.NombreEstado}");
        }

        // Calcular número de factura
        var ultimoNumero = await _context.Facturas.MaxAsync(f => (int?)f.NumeroFactura) ?? 0;

        // Calcular impuestos (ejemplo: 15% IVA)
        var tasaImpuesto = 0.15m;
        var subtotal = orden.Subtotal;
        var montoImpuesto = (subtotal - descuento) * tasaImpuesto;
        var total = (subtotal - descuento) + montoImpuesto;

        // Crear la factura
        var factura = new Factura
        {
            OrdenId = ordenId,
            ClienteId = clienteId ?? orden.ClienteId,
            NumeroFactura = ultimoNumero + 1,
            Subtotal = subtotal,
            Descuento = descuento,
            MontoImpuesto = montoImpuesto,
            MontoTotal = total,
            MetodoPago = string.Empty,
            FechaPago = DateTime.Now,
            Estado = "PENDIENTE"
        };

        _context.Facturas.Add(factura);

        // Copiar detalles de la orden a la factura
        foreach (var detalle in orden.Detalles)
        {
            var detalleFactura = new DetalleFactura
            {
                FacturaId = factura.FacturaId,
                ItemId = detalle.ItemMenuId,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Subtotal = detalle.Subtotal
            };
            _context.DetalleFacturas.Add(detalleFactura);
        }

        // Actualizar orden
        orden.Descuento = descuento;
        orden.MontoImpuesto = montoImpuesto;
        orden.Total = total;
        orden.Estado = EstadoOrden.FACTURADA;
        orden.FechaHoraModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return factura;
    }

    /// <summary>
    /// Registra el pago y cierra la orden
    /// </summary>
    public async Task<Orden> RegistrarPagoYCerrar(int ordenId, string metodoPago)
    {
        var orden = await ObtenerOrdenPorId(ordenId);
        if (orden == null)
        {
            throw new InvalidOperationException($"La orden {ordenId} no existe");
        }

        if (orden.Estado != EstadoOrden.FACTURADA)
        {
            throw new InvalidOperationException("La orden debe estar facturada para registrar el pago");
        }

        if (orden.Factura == null)
        {
            throw new InvalidOperationException("La orden no tiene factura");
        }

        // Actualizar factura
        orden.Factura.MetodoPago = metodoPago;
        orden.Factura.FechaPago = DateTime.Now;
        orden.Factura.Estado = "PAGADA";

        // Cerrar orden
        orden.Estado = EstadoOrden.CERRADA;
        orden.FechaHoraModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return orden;
    }

    /// <summary>
    /// Cancela una orden (solo si no ha sido pagada)
    /// </summary>
    public async Task<Orden> CancelarOrden(int ordenId, string motivo)
    {
        var orden = await ObtenerOrdenPorId(ordenId);
        if (orden == null)
        {
            throw new InvalidOperationException($"La orden {ordenId} no existe");
        }

        if (orden.Estado == EstadoOrden.CERRADA)
        {
            throw new InvalidOperationException("No se puede cancelar una orden cerrada");
        }

        if (orden.Factura != null && orden.Factura.Estado == "PAGADA")
        {
            throw new InvalidOperationException("No se puede cancelar una orden ya pagada");
        }

        orden.Estado = EstadoOrden.CANCELADA;
        orden.Notas = $"CANCELADA: {motivo}. {orden.Notas}";
        orden.FechaHoraModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return orden;
    }

    // ==================== CÁLCULOS ====================

    /// <summary>
    /// Recalcula los totales de una orden basándose en sus detalles
    /// </summary>
    private async Task RecalcularTotales(int ordenId)
    {
        var orden = await _context.Ordenes
            .Include(o => o.Detalles)
            .FirstOrDefaultAsync(o => o.OrdenId == ordenId);

        if (orden == null) return;

        // Calcular subtotal
        orden.Subtotal = orden.Detalles.Sum(d => d.Subtotal);

        // Calcular impuesto
        var tasaImpuesto = 0.15m; // 15% IVA
        orden.MontoImpuesto = orden.Subtotal * tasaImpuesto;

        // Calcular total
        orden.Total = orden.Subtotal + orden.MontoImpuesto - orden.Descuento;

        _context.Ordenes.Update(orden);
    }

    /// <summary>
    /// Obtiene el siguiente número de orden disponible
    /// </summary>
    public async Task<int> ObtenerSiguienteNumeroOrden()
    {
        var ultimoNumero = await _context.Ordenes.MaxAsync(o => (int?)o.NumeroOrden) ?? 0;
        return ultimoNumero + 1;
    }
}