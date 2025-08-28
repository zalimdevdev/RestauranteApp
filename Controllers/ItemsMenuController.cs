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
    public class ItemsMenuController : Controller
    {
        private readonly AppDbContext _context;

        public ItemsMenuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ItemsMenu
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ItemsMenu.Include(i => i.CategoriaMenu);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ItemsMenu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenu = await _context.ItemsMenu
                .Include(i => i.CategoriaMenu)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (itemMenu == null)
            {
                return NotFound();
            }

            return View(itemMenu);
        }

        // GET: ItemsMenu/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriasMenu, "CategoriaId", "NombreCategoria");
            return View();
        }

        // POST: ItemsMenu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,NombreItem,Descripcion,Precio,CategoriaId,Estado")] ItemMenu itemMenu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriasMenu, "CategoriaId", "NombreCategoria", itemMenu.CategoriaId);
            return View(itemMenu);
        }

        // GET: ItemsMenu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenu = await _context.ItemsMenu.FindAsync(id);
            if (itemMenu == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriasMenu, "CategoriaId", "NombreCategoria", itemMenu.CategoriaId);
            return View(itemMenu);
        }

        // POST: ItemsMenu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,NombreItem,Descripcion,Precio,CategoriaId,Estado")] ItemMenu itemMenu)
        {
            if (id != itemMenu.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemMenuExists(itemMenu.ItemId))
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
            ViewData["CategoriaId"] = new SelectList(_context.CategoriasMenu, "CategoriaId", "NombreCategoria", itemMenu.CategoriaId);
            return View(itemMenu);
        }

        // GET: ItemsMenu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemMenu = await _context.ItemsMenu
                .Include(i => i.CategoriaMenu)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (itemMenu == null)
            {
                return NotFound();
            }

            return View(itemMenu);
        }

        // POST: ItemsMenu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemMenu = await _context.ItemsMenu.FindAsync(id);
            if (itemMenu != null)
            {
                _context.ItemsMenu.Remove(itemMenu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemMenuExists(int id)
        {
            return _context.ItemsMenu.Any(e => e.ItemId == id);
        }
    }
}
