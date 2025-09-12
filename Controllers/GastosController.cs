using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;
using Rotativa.AspNetCore;
using System.Data;
using ClosedXML.Excel;

namespace RestauranteApp.Controllers
{
    public class GastosController : Controller
    {
        private readonly AppDbContext _context;

        public GastosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Gastos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gastos.ToListAsync());
        }

        // GET: Gastos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gasto = await _context.Gastos
                .FirstOrDefaultAsync(m => m.GastoId == id);
            if (gasto == null)
            {
                return NotFound();
            }

            return View(gasto);
        }

        // GET: Gastos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gastos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GastoId,Descripcion,Monto,FechaGasto,CategoriaGasto")] Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gasto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gasto);
        }

        // GET: Gastos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gasto = await _context.Gastos.FindAsync(id);
            if (gasto == null)
            {
                return NotFound();
            }
            return View(gasto);
        }

        // POST: Gastos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GastoId,Descripcion,Monto,FechaGasto,CategoriaGasto")] Gasto gasto)
        {
            if (id != gasto.GastoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gasto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GastoExists(gasto.GastoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gasto);
        }

        // GET: Gastos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gasto = await _context.Gastos
                .FirstOrDefaultAsync(m => m.GastoId == id);
            if (gasto == null)
            {
                return NotFound();
            }

            return View(gasto);
        }





// GET: Gastos/GeneratePdf
public async Task<IActionResult> GeneratePdf()
{
    var gastos = await _context.Gastos.OrderBy(g => g.FechaGasto).ToListAsync();
    
    return new ViewAsPdf("ReporteGastos", gastos)
    {
        FileName = $"Reporte_Gastos_{DateTime.Now:yyyyMMddHHmmss}.pdf",
        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
        PageSize = Rotativa.AspNetCore.Options.Size.A4,
        CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8",
        PageMargins = { Left = 10, Right = 10, Top = 15, Bottom = 15 }
    };
}



















// GET: Gastos/ExportarExcel
public async Task<IActionResult> ExportarExcel()
{
    // Obtener los datos de gastos
    var gastos = await _context.Gastos.OrderBy(g => g.FechaGasto).ToListAsync();
    
    // Crear un nuevo libro de trabajo
    using (var workbook = new XLWorkbook())
    {
        // Agregar una hoja de trabajo
        var worksheet = workbook.Worksheets.Add("Gastos");
        
        // Agregar encabezados
        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Descripción";
        worksheet.Cell(1, 3).Value = "Monto";
        worksheet.Cell(1, 4).Value = "Fecha";
        worksheet.Cell(1, 5).Value = "Categoría";
        
        // Formatear encabezados
        var headerRange = worksheet.Range(1, 1, 1, 5);
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        // Llenar con datos
        for (int i = 0; i < gastos.Count; i++)
        {
            var gasto = gastos[i];
            worksheet.Cell(i + 2, 1).Value = gasto.GastoId;
            worksheet.Cell(i + 2, 2).Value = gasto.Descripcion;
            worksheet.Cell(i + 2, 3).Value = gasto.Monto;
            worksheet.Cell(i + 2, 4).Value = gasto.FechaGasto;
            worksheet.Cell(i + 2, 5).Value = gasto.CategoriaGasto;
        }
        
        // Ajustar el ancho de las columnas al contenido
        worksheet.Columns().AdjustToContents();
        
        // Formatear columna de monto como moneda
        worksheet.Column(3).Style.NumberFormat.Format = "#,##0.00";
        
        // Preparar la respuesta
        using (var stream = new MemoryStream())
        {
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Reporte_Gastos_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }
    }
}



        // POST: Gastos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gasto = await _context.Gastos.FindAsync(id);
            if (gasto != null)
            {
                _context.Gastos.Remove(gasto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GastoExists(int id)
        {
            return _context.Gastos.Any(e => e.GastoId == id);
        }
    }
}
