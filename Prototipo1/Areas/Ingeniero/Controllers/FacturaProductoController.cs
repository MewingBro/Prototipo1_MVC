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
    public class FacturaProductoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FacturaProductoController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? IdFactura)
        {
            List<FacturaProducto> objFacturaProductoLista = _unitOfWork.FacturaProducto
        .GetAllBYID(f => f.IdFactura == IdFactura, includeProperties: "Factura,Producto")
        .ToList();

            ViewBag.IdFactura = IdFactura;

            return View(objFacturaProductoLista);
        }

        [HttpPost]
        public IActionResult TerminarFactura(int? IdFactura)
        {
            List<FacturaProducto> objFacturaProductoLista = _unitOfWork.FacturaProducto
        .GetAllBYID(f => f.IdFactura == IdFactura, includeProperties: "Factura,Producto")
        .ToList();

            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            foreach (var x in objFacturaProductoLista)
            {
                _unitOfWork.FacturaProducto.AddWithInventario(x,idProyecto);
                _unitOfWork.Save();
            }
            var factura = _unitOfWork.Factura.GetID(f => f.IdFactura == IdFactura);
            if (factura != null)
            {
                factura.EstadoFactura = 1; // ← cambia el estado
                _unitOfWork.Factura.Update(factura);
            }
            _unitOfWork.Save();

            string Tipo = "Entrada";

            return RedirectToAction("Index","Factura", new { Tipo = Tipo });
        }

        public IActionResult Upsert(int? IdFactura, int? IdFacturaProducto)
        {

            IEnumerable<SelectListItem> ProductoList = _unitOfWork.Producto.GetAll().Select(u => new SelectListItem
            {
                Text = u.NombreProducto,
                Value = u.IdProducto.ToString()
            });

            FacturaProductoVM FacturaProductoVM = new()
            {
                ProductoList = ProductoList,
                FacturaProducto = new FacturaProducto()

            };

            ViewBag.IdFactura = IdFactura;

            if (IdFacturaProducto == null || IdFacturaProducto == 0)
            {
                //crear
                return View(FacturaProductoVM);
            }
            else
            {
                //update
                FacturaProductoVM.FacturaProducto = _unitOfWork.FacturaProducto.GetID(u => u.IdFacturaProducto == IdFacturaProducto);
                return View(FacturaProductoVM);
            }

        }


        [HttpPost]
        public IActionResult Upsert(FacturaProductoVM FacturaProductoVM)
        {
            if (ModelState.IsValid)
            {
                
                if (FacturaProductoVM.FacturaProducto.IdFacturaProducto == 0)
                {
                    _unitOfWork.FacturaProducto.Add(FacturaProductoVM.FacturaProducto);
                    TempData["success"] = "Detalle de Factura agregado exitosamente";
                    
                }
                else
                {
                    _unitOfWork.FacturaProducto.Update(FacturaProductoVM.FacturaProducto);
                    TempData["success"] = "Detalle de Factura actualizado exitosamente";
                }
                int IdFactura = FacturaProductoVM.FacturaProducto.IdFactura;
                _unitOfWork.Save();

                return RedirectToAction("Index", new { IdFactura = IdFactura });

            }
            else
            {
                
                FacturaProductoVM.FacturaList = _unitOfWork.Factura.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Comentario.ToString(),
                    Value = u.IdFactura.ToString()
                });
                FacturaProductoVM.ProductoList = _unitOfWork.Producto.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreProducto.ToString(),
                    Value = u.IdProducto.ToString()
                });

                return View(FacturaProductoVM);
            }

            

        }

        public IActionResult Borrar(int? IdFacturaProducto, int? IdFactura)
        {
            ViewBag.IdFactura = IdFactura;

            if (IdFacturaProducto == null || IdFacturaProducto == 0)
            {
                return NotFound();
            }

            FacturaProducto? FacturaProducto = _unitOfWork.FacturaProducto.GetID(u => u.IdFacturaProducto == IdFacturaProducto);
            if (FacturaProducto == null)
            {
                return NotFound();
            }
            return View(FacturaProducto);

        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdFacturaProducto, int? IdFactura)
        {
            FacturaProducto? obj = _unitOfWork.FacturaProducto.GetID(u => u.IdFacturaProducto == IdFacturaProducto);
            if (obj == null)
            { 
                return Json( new { success = false, message = "Error al borrar" });
            }


            _unitOfWork.FacturaProducto.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Detalle borrado exitosamente";

            return RedirectToAction("Index", new { IdFactura = IdFactura });

        }
    }
}
