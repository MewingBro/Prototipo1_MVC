using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prototipo1.Data;
using Prototipo1.Models;
using Prototipo1.Models.ViewModels;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using System.ComponentModel;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    [Authorize]
    public class UsuariosProyectosController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UsuariosProyectosController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string? IdUsuario, string? filtroProyecto, int page = 1)
        {
            const int pageSize = 10;

            if (string.IsNullOrEmpty(IdUsuario))
            {
                TempData["error"] = "No se encontró el usuario seleccionado.";
                return RedirectToAction("Index", "Usuarios");
            }

            // Obtiene todos los accesos del usuario incluyendo las relaciones
            var query = _unitOfWork.UsuariosProyectos
                .GetAllBYID(f => f.IdUsuario == IdUsuario, includeProperties: "Usuario,Proyecto")
                .AsQueryable();

            // Filtro con búsqueda parcial e insensible a mayúsculas
            if (!string.IsNullOrWhiteSpace(filtroProyecto))
            {
                string filtroLower = filtroProyecto.Trim().ToLower();
                query = query.Where(f => f.Proyecto.NombreProyecto.ToLower().Contains(filtroLower));
            }

            // Paginación
            int totalItems = query.Count();
            var usuariosProyectos = query
                .OrderBy(f => f.Proyecto.NombreProyecto)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Enviar datos a la vista
            ViewBag.IdUsuario = IdUsuario;
            ViewBag.FiltroProyecto = filtroProyecto;
            ViewBag.PageNumber = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(usuariosProyectos);
        }




        public IActionResult Upsert(string? IdUsuario, int? IdUsuariosProyectos)
        {

            IEnumerable<SelectListItem> ProyectoList = _unitOfWork.Proyecto.GetAll().Select(u => new SelectListItem
            {
                Text = u.NombreProyecto,
                Value = u.IdProyecto.ToString()
            });

            UsuariosProyectosVM UsuariosProyectosVM = new()
            {
                ProyectoList = ProyectoList,
                UsuariosProyectos = new UsuariosProyectos()

            };

            ViewBag.IdUsuario = IdUsuario;

            if (IdUsuariosProyectos == null || IdUsuariosProyectos == 0)
            {
                //crear
                return View(UsuariosProyectosVM);
            }
            else
            {
                //update
                UsuariosProyectosVM.UsuariosProyectos = _unitOfWork.UsuariosProyectos.GetID(u => u.IdUsuariosProyectos == IdUsuariosProyectos);
                return View(UsuariosProyectosVM);
            }

        }


        [HttpPost]
        public IActionResult Upsert(UsuariosProyectosVM UsuariosProyectosVM)
        {
            if (ModelState.IsValid)
            {
                
                if (UsuariosProyectosVM.UsuariosProyectos.IdUsuariosProyectos == 0)
                {
                    _unitOfWork.UsuariosProyectos.Add(UsuariosProyectosVM.UsuariosProyectos);
                    TempData["success"] = "Proyecto añadido correctamente";
                    
                }
                else
                {
                    _unitOfWork.UsuariosProyectos.Update(UsuariosProyectosVM.UsuariosProyectos);
                    TempData["success"] = "Detalle de Usuario actualizado exitosamente";
                }
                string IdUsuario = UsuariosProyectosVM.UsuariosProyectos.IdUsuario;
                _unitOfWork.Save();

                return RedirectToAction("Index", new { IdUsuario = IdUsuario });

            }
            else
            {
                
                UsuariosProyectosVM.ProyectoList = _unitOfWork.Proyecto.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreProyecto.ToString(),
                    Value = u.IdProyecto.ToString()
                });

                return View(UsuariosProyectosVM);
            }

            

        }

        public IActionResult Borrar(int? IdUsuariosProyectos, string? IdUsuario)
        {
            ViewBag.IdUsuario = IdUsuario;

            if (IdUsuariosProyectos == null || IdUsuariosProyectos == 0)
            {
                return NotFound();
            }

            UsuariosProyectos? UsuariosProyectos = _unitOfWork.UsuariosProyectos.GetID(u => u.IdUsuariosProyectos == IdUsuariosProyectos);
            if (UsuariosProyectos == null)
            {
                return NotFound();
            }
            return View(UsuariosProyectos);

        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdUsuariosProyectos, string? IdUsuario)
        {
            UsuariosProyectos? obj = _unitOfWork.UsuariosProyectos.GetID(u => u.IdUsuariosProyectos == IdUsuariosProyectos);
            if (obj == null)
            { 
                return Json( new { success = false, message = "Error al borrar" });
            }


            _unitOfWork.UsuariosProyectos.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Detalle borrado exitosamente";

            return RedirectToAction("Index", new { IdUsuario = IdUsuario });

        }
    }
}
