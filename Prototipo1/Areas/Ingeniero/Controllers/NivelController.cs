using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prototipo1.Data;
using Prototipo1.Migrations;
using Prototipo1.Models;
using Prototipo1.Models.ViewModels;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using System.ComponentModel;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    [Authorize]
    public class NivelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public NivelController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                TempData["Error"] = "Debe seleccionar un proyecto antes de ver los niveles.";
                return RedirectToAction("Index", "Home");
            }

            // Obtiene los niveles del proyecto actual
            var niveles = _unitOfWork.Nivel.GetAllBYID(
                f => f.IdProyecto == idProyecto,
                includeProperties: "Proyecto"
            );

            // Filtro sin importar mayúsculas/minúsculas
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                niveles = niveles.Where(n => n.NombreNivel.ToLower().Contains(searchLower));
            }

            // Total y paginación
            var totalNiveles = niveles.Count();
            var nivelesPaginados = niveles
                .OrderBy(n => n.IdNivel)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Datos auxiliares para la vista
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalNiveles = totalNiveles;
            ViewBag.SearchString = searchString;
            ViewBag.IdProyecto = idProyecto;

            return View(nivelesPaginados);
        }


        public IActionResult Upsert(int? IdNivel)
        {
            IEnumerable<SelectListItem> ProyectoList = _unitOfWork.Proyecto.GetAll().Select(u => new SelectListItem
            {
                Text = u.NombreProyecto,
                Value = u.IdProyecto.ToString()
            }
            );

            NivelVM NivelVM = new()
            {
                ProyectoList = ProyectoList,
                Nivel = new Nivel()

            };
            if (IdNivel == null || IdNivel ==0)
            {
                //crear
                return View(NivelVM);
            }
            else
            {
                //update
                NivelVM.Nivel = _unitOfWork.Nivel.GetID(u=>u.IdNivel == IdNivel);
                return View(NivelVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(NivelVM NivelVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {


                if (NivelVM.Nivel.IdNivel == 0)
                {
                    _unitOfWork.Nivel.Add(NivelVM.Nivel);
                } else
                {
                    _unitOfWork.Nivel.Update(NivelVM.Nivel);
                }

                _unitOfWork.Save();
                TempData["success"] = "Nivel agregado exitosamente";
                return RedirectToAction("Index");
            }
            else
            {
                NivelVM.ProyectoList = _unitOfWork.Proyecto.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreProyecto,
                    Value = u.IdProyecto.ToString()
                });

                return View(NivelVM);
            }

            

        }


        public IActionResult Borrar(int? IdNivel)
        {
            if (IdNivel == null || IdNivel == 0)
            {
                return NotFound();
            }

            Nivel? Nivel = _unitOfWork.Nivel.GetID(u => u.IdNivel == IdNivel);
            if (Nivel == null)
            {
                return NotFound();
            }
            return View(Nivel);
        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdNivel)
        {
            Nivel? obj = _unitOfWork.Nivel.GetID(u => u.IdNivel == IdNivel);
            if (obj == null)
            { 
                return Json( new { success = false, message = "Error al borrar" });
            }

            _unitOfWork.Nivel.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Nivel borrado exitosamente";

            return RedirectToAction("Index");

        }
    }
}
