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
    public class FacturaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FacturaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(string Tipo)
        {

            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            List<Factura> objFacturaLista = _unitOfWork.Factura
                .GetAllBYID(f => f.IdProyecto == idProyecto, includeProperties: "TipoFactura,Proyecto")
                .ToList();


            ViewBag.Tipo = Tipo;

            return View(objFacturaLista);
        }

        public IActionResult Upsert(int? IdFactura, string? Tipo)
        {

            ViewBag.Tipo = Tipo;
            IEnumerable<SelectListItem> TipoFacturaList = _unitOfWork.TipoFactura.GetAll().Select(u => new SelectListItem
            {
                Text = u.NombreTipoFactura,
                Value = u.IdTipoFactura.ToString()
            }
            );


            //ViewBag.FamiliaList = FamiliaList;
            //ViewBag.UnidadList = UnidadList;
            FacturaVM FacturaVM = new()
            {
                TipoFacturaList = TipoFacturaList,
                Factura = new Factura()

            };
            if (IdFactura == null || IdFactura ==0)
            {
                //crear
                return View(FacturaVM);
            }
            else
            {
                //update
                FacturaVM.Factura = _unitOfWork.Factura.GetID(u=>u.IdFactura == IdFactura);
                return View(FacturaVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(FacturaVM FacturaVM, string accion)
        {
            if (ModelState.IsValid)
            {

                if (FacturaVM.Factura.IdFactura == 0)
                {
                    if (accion == "AgregarProductosAhora")
                    {
                        _unitOfWork.Factura.Add(FacturaVM.Factura);
                        _unitOfWork.Save();
                        int IdFactura = FacturaVM.Factura.IdFactura;
                        return RedirectToAction(
                "Index",                 // acción del otro controlador
                "FacturaProducto",       // nombre del otro controller
                new { IdFactura = IdFactura } // parámetro
            );
                    } 
                        _unitOfWork.Factura.Add(FacturaVM.Factura);
                        _unitOfWork.Save();
                        TempData["success"] = "Factura agregada exitosamente";
                        return RedirectToAction("Index");
                    
                    
                } else
                {
                    _unitOfWork.Factura.Update(FacturaVM.Factura);
                    _unitOfWork.Save();
                    TempData["success"] = "Factura actualizada exitosamente";
                    return RedirectToAction("Index");
                }

                
            }
            else
            {
                FacturaVM.TipoFacturaList = _unitOfWork.TipoFactura.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreTipoFactura,
                    Value = u.IdTipoFactura.ToString()
                });
                return View(FacturaVM);
            }

            

        }
        /*
        public IActionResult Editar(int? IdFactura)
        {
            if (IdFactura == null || IdFactura == 0)
            {
                return NotFound();
            }

            Factura? Factura = _unitOfWork.Factura.GetID(u => u.IdFactura == IdFactura);
            if (Factura == null)
            {
                return NotFound();
            }
            return View(Factura);
        }
        */

        [HttpPost]
        public IActionResult Editar(Factura obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Factura.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Factura actualizada exitosamente";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Borrar(int? IdFactura)
        {
            if (IdFactura == null || IdFactura == 0)
            {
                return NotFound();
            }

            Factura? Factura = _unitOfWork.Factura.GetID(u => u.IdFactura == IdFactura);
            if (Factura == null)
            {
                return NotFound();
            }
            return View(Factura);
        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdFactura)
        {
            Factura? obj = _unitOfWork.Factura.GetID(u => u.IdFactura == IdFactura);
            if (obj == null)
            { 
                return Json( new { success = false, message = "Error al borrar" });
            }


            _unitOfWork.Factura.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Factura borrada exitosamente";

            return RedirectToAction("Index");

        }
    }
}
