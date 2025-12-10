using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prototipo1.Models.ViewModels;
using Prototipo1.Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Listar usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();
            var userRolesViewModel = new List<UsuarioConRolesVM>();

            foreach (var user in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userRolesViewModel.Add(new UsuarioConRolesVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userRolesViewModel);
        }

        // Vista para crear o editar
        public async Task<IActionResult> Upsert(string? id)
        {
            var roles = _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                })
                .ToList();

            var model = new UsuarioUpsertVM
            {
                RoleList = roles
            };

            // Si tiene id => editar
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound();

                var userRoles = await _userManager.GetRolesAsync(user);

                model.Id = user.Id;
                model.Email = user.Email;
                model.Role = userRoles.FirstOrDefault();
            }

            return View(model);
        }

        // Guardar cambios (crear o editar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UsuarioUpsertVM model)
        {
            if (!ModelState.IsValid)
            {
                model.RoleList = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                });
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                // Crear usuario nuevo
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["success"] = "Usuario creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                // Editar usuario existente
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return NotFound();

                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }

                // Actualizar rol
                var rolesActuales = await _userManager.GetRolesAsync(user);
                if (!rolesActuales.Contains(model.Role))
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesActuales);
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                // Cambiar contraseña si el campo no está vacío
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var removePass = await _userManager.RemovePasswordAsync(user);
                    if (removePass.Succeeded)
                    {
                        var addPass = await _userManager.AddPasswordAsync(user, model.Password);
                        if (!addPass.Succeeded)
                        {
                            foreach (var error in addPass.Errors)
                                ModelState.AddModelError("", error.Description);
                            return View(model);
                        }
                    }
                    else
                    {
                        foreach (var error in removePass.Errors)
                            ModelState.AddModelError("", error.Description);
                        return View(model);
                    }
                }

                TempData["success"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            model.RoleList = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            });
            return View(model);
        }

        public async Task<IActionResult> Borrar(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Borrar")]
        public async Task<IActionResult> BorrarPOST(IdentityUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "Usuario eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


    }



    // ViewModel para la vista de crear/editar
    public class UsuarioUpsertVM
    {
        public string? Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string? ConfirmPassword { get; set; }

        public string Role { get; set; }

        public IEnumerable<SelectListItem>? RoleList { get; set; }
    }

    public class UsuarioConRolesVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
