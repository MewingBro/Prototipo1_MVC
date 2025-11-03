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
    public class FacturaSalidaProductoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FacturaSalidaProductoController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? IdFactura,int? IdRecinto)
        {
            // Obtener la lista de salidas
            List<FacturaSalidaProducto> objFacturaSalidaProductoLista = _unitOfWork.FacturaSalidaProducto
                .GetAllBYID(f => f.IdFactura == IdFactura, includeProperties: "Factura,Producto")
                .ToList();

            ViewBag.IdFactura = IdFactura;

            // 
            var recinto = _unitOfWork.Recinto.GetID(
                f => f.IdRecinto == IdRecinto
            );

            
                if (recinto.EstadoRecinto == 0)
                {
                    ViewBag.SinPresupuesto = true;
                }
            

            return View(objFacturaSalidaProductoLista);
        }

        [HttpPost]
        public IActionResult TerminarFactura(int? IdFactura)
        {
            var listaSalidas = _unitOfWork.FacturaSalidaProducto
                .GetAllBYID(f => f.IdFactura == IdFactura, includeProperties: "Factura,Producto")
                .ToList();

            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            // Buscar el recinto de la factura
            var factura = _unitOfWork.Factura.GetID(f => f.IdFactura == IdFactura);
            if (factura == null)
                return NotFound();

            int? idRecinto = factura.IdRecinto;

            // Verificar si alguna salida excede su presupuesto
            bool excedePresupuesto = false;
            foreach (var salida in listaSalidas)
            {
                var presupuesto = _unitOfWork.RecintoProducto.GetID(
                    r => r.IdRecinto == idRecinto && r.IdProducto == salida.IdProducto);

                if (presupuesto == null || salida.CantidadDisminuida > presupuesto.ExistenciasActuales)
                {
                    excedePresupuesto = true;
                    break;
                }
            }

            // Si hay exceso, mostrar alerta y no ejecutar cambios
            if (excedePresupuesto)
            {
                TempData["AlertaPresupuesto"] = "Uno o más productos exceden el presupuesto asignado. ¿Desea iniciar una solicitud de cambio?";
                return RedirectToAction("Index", "FacturaSalidaProducto", new { IdFactura = IdFactura, IdRecinto = idRecinto });
            }

            // Si todo está dentro del presupuesto
            foreach (var x in listaSalidas)
            {
                _unitOfWork.FacturaSalidaProducto.RemoveFromInventario(x, idProyecto);
                _unitOfWork.Save();
            }

            factura.EstadoFactura = 1; // ← Cambia el estado
            _unitOfWork.Factura.Update(factura);
            _unitOfWork.Save();

            string Tipo = "Salida";
            return RedirectToAction("Index", "Factura", new { Tipo = Tipo, IdRecinto = idRecinto });
        }


        public IActionResult Upsert(int? IdFactura, int? IdFacturaSalidaProducto)
        {
            //  Obtener la factura con su recinto
            var factura = _unitOfWork.Factura.GetID(
                f => f.IdFactura == IdFactura,
                includeProperties: "Recinto"
            );

            // Si la factura existe, filtrar productos por el recinto
            IEnumerable<SelectListItem> ProductoList = Enumerable.Empty<SelectListItem>();

            if (factura != null && factura.IdRecinto != null)
            {
                // Buscar productos presupuestados para el recinto
                var productosPresupuestados = _unitOfWork.RecintoProducto
                    .GetAllBYID(filter: rp => rp.IdRecinto == factura.IdRecinto, includeProperties: "Producto")
                    .Select(rp => rp.Producto)
                    .Distinct()
                    .ToList();

                ProductoList = productosPresupuestados.Select(p => new SelectListItem
                {
                    Text = p.NombreProducto,
                    Value = p.IdProducto.ToString()
                });
            }

            // Crear el ViewModel
            FacturaSalidaProductoVM FacturaSalidaProductoVM = new()
            {
                ProductoList = ProductoList,
                FacturaSalidaProducto = new FacturaSalidaProducto()
            };

            ViewBag.IdFactura = IdFactura;

            //  Lógica para crear o editar
            if (IdFacturaSalidaProducto == null || IdFacturaSalidaProducto == 0)
            {
                // Crear
                return View(FacturaSalidaProductoVM);
            }
            else
            {
                // Editar
                FacturaSalidaProductoVM.FacturaSalidaProducto = _unitOfWork.FacturaSalidaProducto.GetID(
                    u => u.IdFacturaSalidaProducto == IdFacturaSalidaProducto
                );
                return View(FacturaSalidaProductoVM);
            }
        }


        [HttpPost]
        public IActionResult Upsert(FacturaSalidaProductoVM FacturaSalidaProductoVM)
        {
            if (ModelState.IsValid)
            {
                
                if (FacturaSalidaProductoVM.FacturaSalidaProducto.IdFacturaSalidaProducto == 0)
                {
                    _unitOfWork.FacturaSalidaProducto.Add(FacturaSalidaProductoVM.FacturaSalidaProducto);
                    TempData["success"] = "Detalle de Factura agregado exitosamente";
                    
                }
                else
                {
                    _unitOfWork.FacturaSalidaProducto.Update(FacturaSalidaProductoVM.FacturaSalidaProducto);
                    TempData["success"] = "Detalle de Factura actualizado exitosamente";
                }
                int IdFactura = FacturaSalidaProductoVM.FacturaSalidaProducto.IdFactura;

                _unitOfWork.Save();

                var Recinto = _unitOfWork.Factura.GetID(m => m.IdFactura == IdFactura);

                int? IdRecinto = Recinto.IdRecinto;

                return RedirectToAction("Index", new { IdFactura = IdFactura,  IdRecinto = IdRecinto });

            }
            else
            {
                
                FacturaSalidaProductoVM.FacturaList = _unitOfWork.Factura.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Comentario.ToString(),
                    Value = u.IdFactura.ToString()
                });
                FacturaSalidaProductoVM.ProductoList = _unitOfWork.Producto.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreProducto.ToString(),
                    Value = u.IdProducto.ToString()
                });

                return View(FacturaSalidaProductoVM);
            }

            

        }

        public IActionResult Borrar(int? IdFacturaSalidaProducto, int? IdFactura)
        {
            ViewBag.IdFactura = IdFactura;

            if (IdFacturaSalidaProducto == null || IdFacturaSalidaProducto == 0)
            {
                return NotFound();
            }

            FacturaSalidaProducto? FacturaSalidaProducto = _unitOfWork.FacturaSalidaProducto.GetID(u => u.IdFacturaSalidaProducto == IdFacturaSalidaProducto);
            if (FacturaSalidaProducto == null)
            {
                return NotFound();
            }
            return View(FacturaSalidaProducto);

        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdFacturaSalidaProducto, int? IdFactura)
        {
            FacturaSalidaProducto? obj = _unitOfWork.FacturaSalidaProducto.GetID(u => u.IdFacturaSalidaProducto == IdFacturaSalidaProducto);
            if (obj == null)
            { 
                return Json( new { success = false, message = "Error al borrar" });
            }


            _unitOfWork.FacturaSalidaProducto.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Detalle borrado exitosamente";

            return RedirectToAction("Index", new { IdFactura = IdFactura });

        }
    }
}
