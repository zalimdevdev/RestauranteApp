using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestauranteApp.Models;

namespace RestauranteApp.Services
{
    /// <summary>
    /// Servicio para la gestión de inventario de ingredientes.
    /// </summary>
    public interface IStockService
    {
        /// <summary>
        /// Reduce el stock de un ingrediente al vender un artículo.
        /// </summary>
        /// <param name="ingredienteId">ID del ingrediente</param>
        /// <param name="cantidad">Cantidad a reducir</param>
        /// <param name="facturaId">ID de la factura (opcional)</param>
        /// <param name="detalleFacturaId">ID del detalle de factura (opcional)</param>
        /// <returns>Verdadero si la operación fue exitosa</returns>
        Task<bool> ReducirStockAsync(int ingredienteId, decimal cantidad, int? facturaId = null, int? detalleFacturaId = null);

        /// <summary>
        /// Aumenta el stock de un ingrediente al reabastecer.
        /// </summary>
        /// <param name="ingredienteId">ID del ingrediente</param>
        /// <param name="cantidad">Cantidad a agregar</param>
        /// <param name="observacion">Observación del movimiento (opcional)</param>
        /// <returns>Verdadero si la operación fue exitosa</returns>
        Task<bool> AumentarStockAsync(int ingredienteId, decimal cantidad, string? observacion = null);

        /// <summary>
        /// Verifica si hay stock suficiente para un ingrediente.
        /// </summary>
        /// <param name="ingredienteId">ID del ingrediente</param>
        /// <param name="cantidadNecesaria">Cantidad necesaria</param>
        /// <returns>Verdadero si hay stock suficiente</returns>
        Task<bool> HayStockSuficienteAsync(int ingredienteId, decimal cantidadNecesaria);

        /// <summary>
        /// Obtiene el stock actual de un ingrediente.
        /// </summary>
        /// <param name="ingredienteId">ID del ingrediente</param>
        /// <returns>Stock actual</returns>
        Task<decimal> ObtenerStockActualAsync(int ingredienteId);

        /// <summary>
        /// Obtiene el historial de movimientos de un ingrediente.
        /// </summary>
        /// <param name="ingredienteId">ID del ingrediente</param>
        /// <returns>Lista de movimientos</returns>
        Task<IEnumerable<MovimientoStock>> ObtenerHistorialMovimientosAsync(int ingredienteId);

        /// <summary>
        /// Obtiene todos los ingredientes con su stock actual.
        /// </summary>
        /// <returns>Lista de ingredientes con stock</returns>
        Task<IEnumerable<Ingrediente>> ObtenerIngredientesConStockAsync();
    }
}