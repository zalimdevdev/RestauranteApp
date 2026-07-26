using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;

namespace RestauranteApp.Services
{
    /// <summary>
    /// Implementación del servicio de gestión de inventario.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly AppDbContext _context;

        public StockService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<bool> ReducirStockAsync(int ingredienteId, decimal cantidad, int? facturaId = null, int? detalleFacturaId = null)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(ingredienteId);
            if (ingrediente == null)
                return false;

            if (ingrediente.CantidadStock < cantidad)
                return false; // Stock insuficiente

            ingrediente.CantidadStock -= cantidad;

            var movimiento = new MovimientoStock
            {
                IngredienteId = ingredienteId,
                Tipo = "Salida",
                Cantidad = cantidad,
                FacturaId = facturaId,
                DetalleFacturaId = detalleFacturaId,
                Observacion = facturaId.HasValue ? $"Salida por venta Factura #{facturaId}" : "Salida de inventario"
            };

            _context.MovimientosStock.Add(movimiento);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> AumentarStockAsync(int ingredienteId, decimal cantidad, string? observacion = null)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(ingredienteId);
            if (ingrediente == null)
                return false;

            ingrediente.CantidadStock += cantidad;

            var movimiento = new MovimientoStock
            {
                IngredienteId = ingredienteId,
                Tipo = "Entrada",
                Cantidad = cantidad,
                Observacion = observacion ?? "Reabastecimiento"
            };

            _context.MovimientosStock.Add(movimiento);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> HayStockSuficienteAsync(int ingredienteId, decimal cantidadNecesaria)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(ingredienteId);
            if (ingrediente == null)
                return false;

            return ingrediente.CantidadStock >= cantidadNecesaria;
        }

        /// <inheritdoc/>
        public async Task<decimal> ObtenerStockActualAsync(int ingredienteId)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(ingredienteId);
            return ingrediente?.CantidadStock ?? 0m;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MovimientoStock>> ObtenerHistorialMovimientosAsync(int ingredienteId)
        {
            return await _context.MovimientosStock
                .Where(m => m.IngredienteId == ingredienteId)
                .Include(m => m.Factura)
                .Include(m => m.DetalleFactura)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Ingrediente>> ObtenerIngredientesConStockAsync()
        {
            return await _context.Ingredientes
                .Include(i => i.MovimientosStock)
                .OrderBy(i => i.NombreIngrediente)
                .ToListAsync();
        }
    }
}