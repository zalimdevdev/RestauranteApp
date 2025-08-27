using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;

namespace RestauranteApp.Controllers
{
    public class CategoriaMenusController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriaMenusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaMenus
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoriasMenu.ToListAsync());
        }

        // GET: CategoriaMenus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaMenu = await _context.CategoriasMenu
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriaMenu == null)
            {
                return NotFound();
            }

            return View(categoriaMenu);
        }

        // GET: CategoriaMenus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaMenus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoriaId,NombreCategoria")] CategoriaMenu categoriaMenu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaMenu);
        }

        // GET: CategoriaMenus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaMenu = await _context.CategoriasMenu.FindAsync(id);
            if (categoriaMenu == null)
            {
                return NotFound();
            }
            return View(categoriaMenu);
        }

        // POST: CategoriaMenus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoriaId,NombreCategoria")] CategoriaMenu categoriaMenu)
        {
            if (id != categoriaMenu.CategoriaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaMenuExists(categoriaMenu.CategoriaId))
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
            return View(categoriaMenu);
        }

        // GET: CategoriaMenus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaMenu = await _context.CategoriasMenu
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriaMenu == null)
            {
                return NotFound();
            }

            return View(categoriaMenu);
        }

        // POST: CategoriaMenus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaMenu = await _context.CategoriasMenu.FindAsync(id);
            if (categoriaMenu != null)
            {
                _context.CategoriasMenu.Remove(categoriaMenu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaMenuExists(int id)
        {
            return _context.CategoriasMenu.Any(e => e.CategoriaId == id);
        }
    }
}
