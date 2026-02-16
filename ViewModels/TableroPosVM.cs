using RestauranteApp.Models;
using RestauranteApp.Models.Enums;

namespace RestauranteApp.ViewModels;

/// <summary>
/// ViewModel para el tablero de mesas del POS
/// </summary>
public class TableroPosVM
{
    public List<MesaVM> Mesas { get; set; } = new List<MesaVM>();
    public int TotalMesas { get; set; }
    public int MesasDisponibles { get; set; }
    public int MesasOcupadas { get; set; }
}

/// <summary>
/// ViewModel para una mesa individual
/// </summary>
public class MesaVM
{
    public int MesaId { get; set; }
    public int NumeroMesa { get; set; }
    public int Capacidad { get; set; }
    public string? Ubicacion { get; set; }
    public string Estado { get; set; } = "DISPONIBLE";
    public bool TieneOrdenActiva { get; set; }
    public OrdenVM? OrdenActiva { get; set; }
}

/// <summary>
/// ViewModel para una orden
/// </summary>
public class OrdenVM
{
    public int OrdenId { get; set; }
    public int NumeroOrden { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string NombreEstado { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public decimal Total { get; set; }
    public int CantidadItems { get; set; }
    public List<DetalleOrdenVM> Detalles { get; set; } = new List<DetalleOrdenVM>();
    
    // Propiedades para los botones de acción
    public bool PuedeAgregarItems { get; set; }
    public bool PuedeEnviarACocina { get; set; }
    public bool PuedeMarcarServida { get; set; }
    public bool PuedeSolicitarCuenta { get; set; }
    public bool PuedeFacturar { get; set; }
    public bool PuedeCerrar { get; set; }
    public bool PuedeCancelar { get; set; }
}

/// <summary>
/// ViewModel para un detalle de orden
/// </summary>
public class DetalleOrdenVM
{
    public int DetalleOrdenId { get; set; }
    public string NombreItem { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string? Notas { get; set; }
    public bool EstaServido { get; set; }
}

/// <summary>
/// ViewModel para agregar item a la orden
/// </summary>
public class AgregarItemVM
{
    public int OrdenId { get; set; }
    public int ItemMenuId { get; set; }
    public int Cantidad { get; set; } = 1;
    public string? Notas { get; set; }
}

/// <summary>
/// ViewModel para cambio de estado
/// </summary>
public class CambiarEstadoVM
{
    public int OrdenId { get; set; }
    public EstadoOrden NuevoEstado { get; set; }
    public string? Motivo { get; set; }
}

/// <summary>
/// ViewModel para facturar
/// </summary>
public class FacturarVM
{
    public int OrdenId { get; set; }
    public int? ClienteId { get; set; }
    public decimal Descuento { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel para el menú del POS
/// </summary>
public class MenuPosVM
{
    public List<CategoriaMenu> Categorias { get; set; } = new List<CategoriaMenu>();
    public List<ItemMenu> Items { get; set; } = new List<ItemMenu>();
}