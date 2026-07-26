using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteApp.ViewModels
{
    /// <summary>
    /// ViewModel para el proceso de reabastecimiento de ingredientes.
    /// </summary>
    public class ReabastecimientoVM
    {
        /// <summary>
        /// ID del ingrediente a reabastecer
        /// </summary>
        [Required(ErrorMessage = "El ingrediente es obligatorio")]
        public int IngredienteId { get; set; }

        /// <summary>
        /// Nombre del ingrediente (solo lectura, para mostrar en la vista)
        /// </summary>
        [Display(Name = "Nombre del Ingrediente")]
        public string NombreIngrediente { get; set; } = string.Empty;

        /// <summary>
        /// Stock actual del ingrediente (solo lectura)
        /// </summary>
        [Display(Name = "Stock Actual")]
        public decimal StockActual { get; set; }

        /// <summary>
        /// Unidad de medida del ingrediente (solo lectura)
        /// </summary>
        [Display(Name = "Unidad de Medida")]
        public string UnidadMedida { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad a agregar al stock
        /// </summary>
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0.001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad a Agregar")]
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Fecha del reabastecimiento
        /// </summary>
        [Display(Name = "Fecha del Reabastecimiento")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        /// <summary>
        /// Observaciones del reabastecimiento
        /// </summary>
        [StringLength(250, ErrorMessage = "La observación no puede exceder 250 caracteres")]
        [Display(Name = "Observación")]
        public string? Observacion { get; set; }

        /// <summary>
        /// Stock después del reabastecimiento (calculado)
        /// </summary>
        [Display(Name = "Stock Después del Reabastecimiento")]
        public decimal StockDespues { get; set; }

        // Lista de ingredientes para el dropdown
        public IEnumerable<SelectListItem>? Ingredientes { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar el detalle de un movimiento de stock
    /// </summary>
    public class DetalleMovimientoVM
    {
        public int IngredienteId { get; set; }
        public string NombreIngrediente { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
        public List<MovimientoCantidadVM> Movimientos { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para mostrar un movimiento individual
    /// </summary>
    public class MovimientoCantidadVM
    {
        public int MovimientoId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string? Observacion { get; set; }
        public int? FacturaId { get; set; }
    }
}