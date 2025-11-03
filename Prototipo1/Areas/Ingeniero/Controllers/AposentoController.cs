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
    public class AposentoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AposentoController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(string searchAposento, string searchNivel, int page = 1, int pageSize = 10)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                TempData["Error"] = "Debe seleccionar un proyecto antes de ver los aposentos.";
                return RedirectToAction("Index", "Home");
            }

            // 🔹 Obtiene los aposentos del proyecto actual
            var aposentos = _unitOfWork.Aposento.GetAllBYID(
                a => a.Nivel.IdProyecto == idProyecto,
                includeProperties: "Nivel,Nivel.Proyecto"
            );

            // 🔍 Filtros (insensibles a mayúsculas/minúsculas)
            if (!string.IsNullOrEmpty(searchAposento))
            {
                var aposentoLower = searchAposento.ToLower();
                aposentos = aposentos.Where(a => a.NombreAposento.ToLower().Contains(aposentoLower));
            }

            if (!string.IsNullOrEmpty(searchNivel))
            {
                var nivelLower = searchNivel.ToLower();
                aposentos = aposentos.Where(a => a.Nivel.NombreNivel.ToLower().Contains(nivelLower));
            }

            // 📊 Total y paginación
            var totalAposentos = aposentos.Count();
            var aposentosPaginados = aposentos
                .OrderBy(a => a.IdAposento)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 📦 Datos auxiliares para la vista
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalAposentos = totalAposentos;
            ViewBag.SearchAposento = searchAposento;
            ViewBag.SearchNivel = searchNivel;

            return View(aposentosPaginados);
        }


        public IActionResult Upsert(int? IdAposento)

        {

            // 🔹 Obtener el IdProyecto actual (desde Session o donde lo tengas guardado)
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                // Opcional: puedes redirigir o lanzar un error si no hay proyecto seleccionado
                TempData["Error"] = "Debe seleccionar un proyecto antes de agregar un aposento.";
                return RedirectToAction("Index", "Proyecto");
            }

            // 🔹 Filtrar los niveles solo del proyecto actual
            IEnumerable<SelectListItem> NivelList = _unitOfWork.Nivel
                .GetAllBYID(n => n.IdProyecto == idProyecto)
                .Select(u => new SelectListItem
                {
                    Text = u.NombreNivel,
                    Value = u.IdNivel.ToString()
                });

            // 🔹 Crear el ViewModel
            AposentoVM AposentoVM = new()
            {
                NivelList = NivelList,
                Aposento = new Aposento()
            };

            // 🔹 Si es nuevo registro
            if (IdAposento == null || IdAposento == 0)
            {
                return View(AposentoVM);
            }
            else
            {
                // 🔹 Si es edición, cargar el Aposento existente
                AposentoVM.Aposento = _unitOfWork.Aposento.GetID(u => u.IdAposento == IdAposento);
                return View(AposentoVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(AposentoVM AposentoVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {


                if (AposentoVM.Aposento.IdAposento == 0)
                {
                    _unitOfWork.Aposento.Add(AposentoVM.Aposento);
                }
                else
                {
                    _unitOfWork.Aposento.Update(AposentoVM.Aposento);
                }

                _unitOfWork.Save();
                TempData["success"] = "Aposento agregado exitosamente";
                return RedirectToAction("Index");
            }
            else
            {
                AposentoVM.NivelList = _unitOfWork.Nivel.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreNivel,
                    Value = u.IdNivel.ToString()
                });

                return View(AposentoVM);
            }



        }
        /*
        public IActionResult Editar(int? IdAposento)
        {
            if (IdAposento == null || IdAposento == 0)
            {
                return NotFound();
            }

            Aposento? Aposento = _unitOfWork.Aposento.GetID(u => u.IdAposento == IdAposento);
            if (Aposento == null)
            {
                return NotFound();
            }
            return View(Aposento);
        }
        */


        public IActionResult Borrar(int? IdAposento)
        {
            if (IdAposento == null || IdAposento == 0)
            {
                return NotFound();
            }

            Aposento? Aposento = _unitOfWork.Aposento.GetID(u => u.IdAposento == IdAposento);
            if (Aposento == null)
            {
                return NotFound();
            }
            return View(Aposento);
        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdAposento)
        {
            Aposento? obj = _unitOfWork.Aposento.GetID(u => u.IdAposento == IdAposento);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error al borrar" });
            }

            _unitOfWork.Aposento.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Aposento borrado exitosamente";

            return RedirectToAction("Index");

        }
    }
}
