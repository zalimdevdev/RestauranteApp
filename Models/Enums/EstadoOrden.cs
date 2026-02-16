namespace RestauranteApp.Models.Enums;

/// <summary>
/// Estados de una orden en el sistema POS
/// </summary>
public enum EstadoOrden
{
    /// <summary>Orden creada, esperando preparación</summary>
    ABIERTA = 1,
    
    /// <summary>Orden en proceso de preparación en cocina</summary>
    EN_PREPARACION = 2,
    
    /// <summary>Orden lista y servida al cliente</summary>
    SERVIDA = 3,
    
    /// <summary>Cliente solicita la cuenta</summary>
    SOLICITA_CUENTA = 4,
    
    /// <summary>Orden facturada, esperando pago</summary>
    FACTURADA = 5,
    
    /// <summary>Orden pagada y cerrada completamente</summary>
    CERRADA = 6,
    
    /// <summary>Orden cancelada (sin posibilidad de recuperación)</summary>
    CANCELADA = 7
}