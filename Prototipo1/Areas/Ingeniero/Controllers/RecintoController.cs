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
    public class RecintoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RecintoController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(string searchRecinto, string searchAposento, string searchNivel, int page = 1, int pageSize = 10)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                TempData["Error"] = "Debe seleccionar un proyecto antes de ver los recintos.";
                return RedirectToAction("Index", "Home");
            }

            var recintos = _unitOfWork.Recinto.GetAllBYID(
                a => a.Aposento.Nivel.IdProyecto == idProyecto,
                includeProperties: "Aposento,Aposento.Nivel.Proyecto"
            ).AsQueryable();

            // 🔍 Filtros (case-insensitive)
            if (!string.IsNullOrEmpty(searchRecinto))
            {
                var recintoLower = searchRecinto.ToLower();
                recintos = recintos.Where(r => r.NombreRecinto.ToLower().Contains(recintoLower));
            }

            if (!string.IsNullOrEmpty(searchAposento))
            {
                var aposentoLower = searchAposento.ToLower();
                recintos = recintos.Where(r => r.Aposento.NombreAposento.ToLower().Contains(aposentoLower));
            }

            if (!string.IsNullOrEmpty(searchNivel))
            {
                var nivelLower = searchNivel.ToLower();
                recintos = recintos.Where(r => r.Aposento.Nivel.NombreNivel.ToLower().Contains(nivelLower));
            }

            // 📄 Paginación
            var total = recintos.Count();
            var recintosPaginados = recintos
                .OrderBy(r => r.IdRecinto)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecintos = total;
            ViewBag.SearchRecinto = searchRecinto;
            ViewBag.SearchAposento = searchAposento;
            ViewBag.SearchNivel = searchNivel;

            return View(recintosPaginados);
        }


        public IActionResult Upsert(int? IdRecinto)

        {

            // 🔹 Obtener el IdProyecto actual (desde Session o donde lo tengas guardado)
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                // Opcional: puedes redirigir o lanzar un error si no hay proyecto seleccionado
                TempData["Error"] = "Debe seleccionar un proyecto antes de agregar un Recinto.";
                return RedirectToAction("Index", "Proyecto");
            }

            // 🔹 Filtrar los niveles solo del proyecto actual
            IEnumerable<SelectListItem> AposentoList = _unitOfWork.Aposento
                .GetAllBYID(n => n.Nivel.IdProyecto == idProyecto)
                .Select(u => new SelectListItem
                {
                    Text = u.NombreAposento,
                    Value = u.IdAposento.ToString()
                });

            // 🔹 Crear el ViewModel
            RecintoVM RecintoVM = new()
            {
                AposentoList = AposentoList,
                Recinto = new Recinto()
            };

            // 🔹 Si es nuevo registro
            if (IdRecinto == null || IdRecinto == 0)
            {
                return View(RecintoVM);
            }
            else
            {
                // 🔹 Si es edición, cargar el Recinto existente
                RecintoVM.Recinto = _unitOfWork.Recinto.GetID(u => u.IdRecinto == IdRecinto);
                return View(RecintoVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(RecintoVM RecintoVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {


                if (RecintoVM.Recinto.IdRecinto == 0)
                {
                    _unitOfWork.Recinto.Add(RecintoVM.Recinto);
                }
                else
                {
                    _unitOfWork.Recinto.Update(RecintoVM.Recinto);
                }

                _unitOfWork.Save();
                TempData["success"] = "Recinto agregado exitosamente";
                return RedirectToAction("Index");
            }
            else
            {
                RecintoVM.AposentoList = _unitOfWork.Aposento.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreAposento,
                    Value = u.IdAposento.ToString()
                });

                return View(RecintoVM);
            }



        }
        /*
        public IActionResult Editar(int? IdRecinto)
        {
            if (IdRecinto == null || IdRecinto == 0)
            {
                return NotFound();
            }

            Recinto? Recinto = _unitOfWork.Recinto.GetID(u => u.IdRecinto == IdRecinto);
            if (Recinto == null)
            {
                return NotFound();
            }
            return View(Recinto);
        }
        */


        public IActionResult Borrar(int? IdRecinto)
        {
            if (IdRecinto == null || IdRecinto == 0)
            {
                return NotFound();
            }

            Recinto? Recinto = _unitOfWork.Recinto.GetID(u => u.IdRecinto == IdRecinto);
            if (Recinto == null)
            {
                return NotFound();
            }
            return View(Recinto);
        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdRecinto)
        {
            Recinto? obj = _unitOfWork.Recinto.GetID(u => u.IdRecinto == IdRecinto);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error al borrar" });
            }

            _unitOfWork.Recinto.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Recinto borrado exitosamente";

            return RedirectToAction("Index");

        }
    }
}
