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


        public IActionResult Index()
        {
            List<Familia> objFamiliaLista = _unitOfWork.Familia.GetAll().ToList();
            return View(objFamiliaLista);
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
