using Microsoft.AspNetCore.Mvc;
using Prototipo1.Data;
using Prototipo1.Models;
using System.ComponentModel;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    public class UnidadController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnidadController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Unidad> objUnidadLista = _unitOfWork.Unidad.GetAll().ToList();
            return View(objUnidadLista);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Unidad obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Unidad.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Unidad agregada exitosamente";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Editar(int? IdUnidad)
        {
            if (IdUnidad == null || IdUnidad == 0)
            {
                return NotFound();
            }

            Unidad? Unidad = _unitOfWork.Unidad.GetID(u => u.IdUnidad == IdUnidad);
            if (Unidad == null)
            {
                return NotFound();
            }
            return View(Unidad);
        }

        [HttpPost]
        public IActionResult Editar(Unidad obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Unidad.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Unidad actualizada exitosamente";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Borrar(int? IdUnidad)
        {
            if (IdUnidad == null || IdUnidad == 0)
            {
                return NotFound();
            }

            Unidad? Unidad = _unitOfWork.Unidad.GetID(u => u.IdUnidad == IdUnidad);
            if (Unidad == null)
            {
                return NotFound();
            }
            return View(Unidad);
        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdUnidad)
        {
            Unidad? obj = _unitOfWork.Unidad.GetID(u => u.IdUnidad == IdUnidad);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Unidad.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Unidad borrada exitosamente";
            return RedirectToAction("Index");

        }
    }
}
