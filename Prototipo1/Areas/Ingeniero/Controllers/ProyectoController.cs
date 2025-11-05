using Microsoft.AspNetCore.Mvc;
using Prototipo1.Data;
using Prototipo1.Models;
using System.ComponentModel;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prototipo1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Prototipo1.Utility;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProyectoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProyectoController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(string searchNombre, int page = 1)
        {
            const int pageSize = 10; // Número de registros por página

            // Consulta base
            var proyectos = _unitOfWork.Proyecto.GetAll();

            // Filtro por nombre
            if (!string.IsNullOrWhiteSpace(searchNombre))
            {
                proyectos = proyectos.Where(p =>
                    p.NombreProyecto.ToLower().Contains(searchNombre.ToLower()));
            }

            // Total de registros para paginación
            int totalProyectos = proyectos.Count();

            // Obtener la página actual
            var proyectosPaginados = proyectos
                .OrderBy(p => p.IdProyecto)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Datos de paginación para la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalProyectos / (double)pageSize);
            ViewBag.SearchNombre = searchNombre;

            return View(proyectosPaginados);
        }

        public IActionResult Upsert(int? IdProyecto)
        {

            //ViewBag.FamiliaList = FamiliaList;
            //ViewBag.UnidadList = UnidadList;
            Proyecto proyecto = new Proyecto();

            if (IdProyecto == null || IdProyecto ==0)
            {
                //crear
                return View(proyecto);
            }
            else
            {
                //update
                Proyecto proyectoID = _unitOfWork.Proyecto.GetID(u=>u.IdProyecto == IdProyecto);
                return View(proyectoID);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(Proyecto Proyecto, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null) { 
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProyectoPATH = Path.Combine(wwwRootPath, @"Images\Proyecto");

                    if (!string.IsNullOrEmpty(Proyecto.ImageURL))
                    {
                        //borra la imagen anterior
                        var oldImagePath = Path.Combine(wwwRootPath, Proyecto.ImageURL.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    

                    using (var fileStream = new FileStream(Path.Combine(ProyectoPATH, fileName), FileMode.Create)) { 
                        file.CopyTo(fileStream);
                    }
                    Proyecto.ImageURL = @"\Images\Proyecto\" + fileName;
                }

                if (Proyecto.IdProyecto == 0)
                {
                    _unitOfWork.Proyecto.Add(Proyecto);
                } else
                {
                    _unitOfWork.Proyecto.Update(Proyecto);
                }

                _unitOfWork.Save();
                TempData["success"] = "Proyecto agregado exitosamente";
                return RedirectToAction("Index");
            }
            else
            {
                return View(Proyecto);
            }

            

        }


        public IActionResult Borrar(int? IdProyecto)
        {
            if (IdProyecto == null || IdProyecto == 0)
            {
                return NotFound();
            }

            Proyecto? Proyecto = _unitOfWork.Proyecto.GetID(u => u.IdProyecto == IdProyecto);
            if (Proyecto == null)
            {
                return NotFound();
            }
            return View(Proyecto);
        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdProyecto)
        {
            Proyecto? obj = _unitOfWork.Proyecto.GetID(u => u.IdProyecto == IdProyecto);
            if (obj == null)
            { 
                return Json( new { success = false, message = "Error al borrar" });
            }

            var imagenBorrar = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageURL.TrimStart('\\'));

            if (System.IO.File.Exists(imagenBorrar))
            {
                System.IO.File.Delete(imagenBorrar);
            }
            _unitOfWork.Proyecto.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Proyecto borrado exitosamente";

            return RedirectToAction("Index");

        }
    }
}
