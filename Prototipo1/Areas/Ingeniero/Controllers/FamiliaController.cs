using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prototipo1.Data;
using Prototipo1.Models;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using System.ComponentModel;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    [Authorize]
    public class FamiliaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public FamiliaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            // Obtener todas las familias
            var familias = _unitOfWork.Familia.GetAll();

            // Filtrado sin importar mayúsculas/minúsculas
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                familias = familias.Where(f => f.NombreFamilia.ToLower().Contains(searchLower));
            }

            // Total de resultados y paginación
            var totalFamilias = familias.Count();
            var familiasPaginadas = familias
                .OrderBy(f => f.IdFamilia)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pasar datos a la vista
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalFamilias = totalFamilias;
            ViewBag.SearchString = searchString;

            return View(familiasPaginadas);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Familia obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Familia.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Familia agregada exitosamente";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Editar(int? IdFamilia)
        {
            if (IdFamilia == null || IdFamilia == 0)
            {
                return NotFound();
            }

            Familia? familia = _unitOfWork.Familia.GetID(u => u.IdFamilia == IdFamilia);
            if (familia == null)
            {
                return NotFound();
            }
            return View(familia);
        }

        [HttpPost]
        public IActionResult Editar(Familia obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Familia.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Familia actualizada exitosamente";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Borrar(int? IdFamilia)
        {
            if (IdFamilia == null || IdFamilia == 0)
            {
                return NotFound();
            }

            Familia? familia = _unitOfWork.Familia.GetID(u => u.IdFamilia == IdFamilia);
            if (familia == null)
            {
                return NotFound();
            }
            return View(familia);
        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdFamilia)
        {
            Familia? obj = _unitOfWork.Familia.GetID(u => u.IdFamilia == IdFamilia);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Familia.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Familia borrada exitosamente";
            return RedirectToAction("Index");

        }
    }
}
