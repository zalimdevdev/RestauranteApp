using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace RestauranteApp.Controllers
{
    public class UserRolController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRolController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
   
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.ToListAsync();
            var userListVM = new UserListVM
            {
                Usuarios = new List<UserRolVM>(),
                Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name")
            };

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                var rolActual = roles.FirstOrDefault() ?? "Sin rol";
                
                userListVM.Usuarios.Add(new UserRolVM
                {
                    UserId = usuario.Id,
                    UserName = usuario.UserName ?? "",
                    Email = usuario.Email ?? "",
                    RolActual = rolActual,
                    LockoutEnabled = usuario.LockoutEnabled,
                    EmailConfirmed = usuario.EmailConfirmed,
                    LockoutEnd = usuario.LockoutEnd
                });
            }

            return View(userListVM);
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Json(new { data = _userManager.Users.ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> CambiarRol(string userId, string nuevoRol)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(nuevoRol))
            {
                return Json(new { success = false, message = "Datos inválidos" });
            }

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            if (rolesActuales.Any())
            {
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            }

            var resultado = await _userManager.AddToRoleAsync(usuario, nuevoRol);
            if (resultado.Succeeded)
            {
                return Json(new { success = true, message = $"Rol actualizado a {nuevoRol}" });
            }

            return Json(new { success = false, message = "Error al actualizar rol" });
        }

        [HttpPost]
        public async Task<IActionResult> SuspenderUsuario(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Usuario no válido" });
            }

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            usuario.LockoutEnabled = true;
            usuario.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                return Json(new { success = true, message = "Usuario suspendido" });
            }

            return Json(new { success = false, message = "Error al suspender usuario" });
        }

        [HttpPost]
        public async Task<IActionResult> ActivarUsuario(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Usuario no válido" });
            }

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            usuario.LockoutEnabled = false;
            usuario.LockoutEnd = null;
            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                return Json(new { success = true, message = "Usuario activado" });
            }

            return Json(new { success = false, message = "Error al activar usuario" });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Usuario no válido" });
            }

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            var resultado = await _userManager.DeleteAsync(usuario);
            if (resultado.Succeeded)
            {
                return Json(new { success = true, message = "Usuario eliminado" });
            }

            return Json(new { success = false, message = "Error al eliminar usuario" });
        }

        [HttpPost]
        public async Task<IActionResult> AddRol(string rolName)
        {
            if (string.IsNullOrEmpty(rolName))
            {
                TempData["Error"] = "El nombre del rol es obligatorio";
                return RedirectToAction("Index");
            }

            var existeRol = await _roleManager.RoleExistsAsync(rolName);
            if (!existeRol)
            {
                var resultado = await _roleManager.CreateAsync(new IdentityRole(rolName));
                if (!resultado.Succeeded)
                {
                    TempData["Error"] = "Error al crear el rol";
                    return RedirectToAction("Index");
                }
            }

            TempData["Success"] = $"Rol '{rolName}' creado exitosamente";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddUserRol(string UserId, string RoleId)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(RoleId))
            {
                TempData["Error"] = "Debe seleccionar un usuario y un rol";
                return RedirectToAction("Index");
            }

            var usuario = await _userManager.FindByIdAsync(UserId);
            var rol = await _roleManager.FindByIdAsync(RoleId);

            if (usuario == null || rol == null)
            {
                TempData["Error"] = "Usuario o rol no encontrado";
                return RedirectToAction("Index");
            }

            var resultado = await _userManager.AddToRoleAsync(usuario, rol.Name!);
            if (resultado.Succeeded)
            {
                TempData["Success"] = $"Rol '{rol.Name}' asignado a '{usuario.UserName}'";
            }
            else
            {
                TempData["Error"] = "Error al asignar el rol";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerRoles()
        {
            var roles = await _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToListAsync();
            return Json(roles);
        }
    }
}
