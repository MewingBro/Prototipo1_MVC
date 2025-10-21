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
    public class RecintoProductoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RecintoProductoController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? IdRecinto)
        {
            List<RecintoProducto> objRecintoProductoLista = _unitOfWork.RecintoProducto
        .GetAllBYID(f => f.IdRecinto == IdRecinto, includeProperties: "Recinto,Producto")
        .ToList();

            ViewBag.IdRecinto = IdRecinto;

            return View(objRecintoProductoLista);
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

            return RedirectToAction("Index", "Factura");
        }

        public IActionResult Upsert(int? IdRecinto, int? IdRecintoProducto)
        {

            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                return RedirectToAction("SeleccionarProyecto", "Proyecto");
            }

            // 1️⃣ Obtener los productos del inventario del proyecto
            var inventarios = _unitOfWork.Inventario
                .GetAllBYID(i => i.IdProyecto == idProyecto)
                .ToList();

            // 2️⃣ Extraer sus IdProducto
            var idsProductosInventario = inventarios
                .Select(i => i.IdProducto)
                .ToList();

            // 3️⃣ Obtener todos los productos y luego filtrar en memoria
            var productos = _unitOfWork.Producto
                .GetAll()
                .Where(p => idsProductosInventario.Contains(p.IdProducto))
                .ToList();

            // 4️⃣ Armar la lista para el dropdown
            IEnumerable<SelectListItem> ProductoList = productos.Select(u => new SelectListItem
            {
                Text = u.NombreProducto,
                Value = u.IdProducto.ToString()
            });

            RecintoProductoVM RecintoProductoVM = new()
            {
                ProductoList = ProductoList,
                RecintoProducto = new RecintoProducto()
            };

            ViewBag.IdRecinto = IdRecinto;

            if (IdRecintoProducto == null || IdRecintoProducto == 0)
            {
                return View(RecintoProductoVM);
            }
            else
            {
                RecintoProductoVM.RecintoProducto = _unitOfWork.RecintoProducto.GetID(u => u.IdRecintoProducto == IdRecintoProducto);
                return View(RecintoProductoVM);
            }

        }


        [HttpPost]
        public IActionResult Upsert(RecintoProductoVM RecintoProductoVM)
        {
            if (ModelState.IsValid)
            {
                
                if (RecintoProductoVM.RecintoProducto.IdRecintoProducto == 0)
                {
                    _unitOfWork.RecintoProducto.Add(RecintoProductoVM.RecintoProducto);
                    TempData["success"] = "Presupuesto agregado exitosamente";
                    
                }
                else
                {
                    _unitOfWork.RecintoProducto.Update(RecintoProductoVM.RecintoProducto);
                    TempData["success"] = "Presupuesto actualizado exitosamente";
                }
                int IdRecinto = RecintoProductoVM.RecintoProducto.IdRecinto;
                _unitOfWork.Save();

                return RedirectToAction("Index", new { IdRecinto = IdRecinto });

            }
            else
            {

                RecintoProductoVM.RecintoList = _unitOfWork.Recinto.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreRecinto.ToString(),
                    Value = u.IdRecinto.ToString()
                });
                RecintoProductoVM.ProductoList = _unitOfWork.Producto.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreProducto.ToString(),
                    Value = u.IdProducto.ToString()
                });

                return View(RecintoProductoVM);
            }

            

        }

        public IActionResult Borrar(int? IdRecintoProducto, int? IdRecinto)
        {
            ViewBag.IdRecinto = IdRecinto;

            if (IdRecintoProducto == null || IdRecintoProducto == 0)
            {
                return NotFound();
            }

            RecintoProducto? RecintoProducto = _unitOfWork.RecintoProducto.GetID(u => u.IdRecintoProducto == IdRecintoProducto);
            if (RecintoProducto == null)
            {
                return NotFound();
            }
            return View(RecintoProducto);

        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdRecintoProducto, int? IdRecinto)
        {
            RecintoProducto? obj = _unitOfWork.RecintoProducto.GetID(u => u.IdRecintoProducto == IdRecintoProducto);
            if (obj == null)
            { 
                return Json( new { success = false, message = "Error al borrar" });
            }


            _unitOfWork.RecintoProducto.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Presupuesto borrado exitosamente, unidades devueltas";

            return RedirectToAction("Index", new { IdRecinto = IdRecinto });

        }
    }
}
