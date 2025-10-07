using Microsoft.AspNetCore.Mvc;
using Prototipo1.Data;
using Prototipo1.Models;
using System.ComponentModel;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prototipo1.Models.ViewModels;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    public class NivelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public NivelController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Nivel> objNivelLista = _unitOfWork.Nivel.GetAll(includeProperties:"Proyecto").ToList();


            return View(objNivelLista);
        }

        public IActionResult Upsert(int? IdNivel)
        {
            IEnumerable<SelectListItem> ProyectoList = _unitOfWork.Proyecto.GetAll().Select(u => new SelectListItem
            {
                Text = u.NombreProyecto,
                Value = u.IdProyecto.ToString()
            }
            );

            //ViewBag.FamiliaList = FamiliaList;
            //ViewBag.UnidadList = UnidadList;
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
        /*
        public IActionResult Editar(int? IdNivel)
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
        */


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
