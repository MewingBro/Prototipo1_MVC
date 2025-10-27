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

            // Determinar el IdTipoFactura según el tipo recibido
            int? idTipoFactura = null;
            if (!string.IsNullOrEmpty(Tipo))
            {
                if (Tipo.Equals("Entrada", StringComparison.OrdinalIgnoreCase))
                    idTipoFactura = 1;
                else if (Tipo.Equals("Salida", StringComparison.OrdinalIgnoreCase))
                    idTipoFactura = 2;
            }

            // Consulta base filtrando por proyecto
            var query = _unitOfWork.Factura.GetAllBYID(
                f => f.IdProyecto == idProyecto,
                includeProperties: "TipoFactura,Proyecto,Recinto,Recinto.Aposento.Nivel"
            );

            // Si se definió un tipo, aplicar filtro adicional
            if (idTipoFactura.HasValue)
            {
                query = query.Where(f => f.IdTipoFactura == idTipoFactura.Value);
            }

            var objFacturaLista = query.ToList();

            ViewBag.Tipo = Tipo;

            return View(objFacturaLista);
        }

        public IActionResult Upsert(int? IdFactura, string? Tipo)
        {
            ViewBag.Tipo = Tipo;
            ViewBag.IdFactura = IdFactura;

            // Lista de tipos de factura
            IEnumerable<SelectListItem> TipoFacturaList = _unitOfWork.TipoFactura.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.NombreTipoFactura,
                    Value = u.IdTipoFactura.ToString()
                });

            // ✅ Lista de recintos filtrados por proyecto (si hay proyecto seleccionado)
            IEnumerable<SelectListItem> RecintoList = new List<SelectListItem>();
            int? IdProyecto = HttpContext.Session.GetInt32("IdProyecto");
            if (IdProyecto != null && IdProyecto != 0)
            {
                RecintoList = _unitOfWork.Recinto.GetAllBYID(
                    filter: r => r.Aposento.Nivel.Proyecto.IdProyecto == IdProyecto,
                    includeProperties: "Aposento" // necesario si usas navegación
                )
                .Select(r => new SelectListItem
                {
                    Text = r.NombreRecinto,    // ajusta al nombre real del campo
                    Value = r.IdRecinto.ToString()
                });
            }

            FacturaVM FacturaVM = new()
            {
                TipoFacturaList = TipoFacturaList,
                RecintoList = RecintoList, // ✅ Se agrega al ViewModel
                Factura = new Factura()
            };

            if (IdFactura == null || IdFactura == 0)
            {
                // Crear
                return View(FacturaVM);
            }
            else
            {
                // Editar
                FacturaVM.Factura = _unitOfWork.Factura.GetID(u => u.IdFactura == IdFactura);
                // Opcional: volver a cargar recintos del proyecto asociado a la factura
                if (FacturaVM.Factura.IdProyecto != 0)
                {
                    FacturaVM.RecintoList = _unitOfWork.Recinto.GetAllBYID(
                        filter: r => r.Aposento.Nivel.Proyecto.IdProyecto == IdProyecto,
                        includeProperties: "Aposento"
                    ).Select(r => new SelectListItem
                    {
                        Text = r.NombreRecinto,
                        Value = r.IdRecinto.ToString()
                    });
                }
                return View(FacturaVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(FacturaVM FacturaVM, string accion)
        {
            if (ModelState.IsValid)
            {
                string Tipo;
                int TipoNum = FacturaVM.Factura.IdTipoFactura;
                if (TipoNum == 1)
                {
                    Tipo = "Entrada";
                }
                else
                {
                    Tipo = "Salida";
                }
                if (FacturaVM.Factura.IdFactura == 0)
                {
                    if (accion == "AgregarProductosAhora")
                    {
                        _unitOfWork.Factura.Add(FacturaVM.Factura);
                        _unitOfWork.Save();
                        int IdFactura = FacturaVM.Factura.IdFactura;
                        if (TipoNum == 1)
                        {
                            return RedirectToAction(
                "Index",                 // acción del otro controlador
                "FacturaProducto",       // nombre del otro controller
                new { IdFactura = IdFactura } // parámetro
            );
                        }
                        else
                        {
                            return RedirectToAction(
                                            "Index",                 // acción del otro controlador
                                            "FacturaSalidaProducto",       // nombre del otro controller
                                            new { IdFactura = IdFactura } // parámetro
                                        );

                        }

                    }
                    _unitOfWork.Factura.Add(FacturaVM.Factura);
                    _unitOfWork.Save();
                    TempData["success"] = "Factura agregada exitosamente";
                    return RedirectToAction("Index", new { Tipo = Tipo });


                }
                else
                {
                    _unitOfWork.Factura.Update(FacturaVM.Factura);
                    _unitOfWork.Save();
                    TempData["success"] = "Factura actualizada exitosamente";
                    return RedirectToAction("Index", new { Tipo = Tipo });
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
                return Json(new { success = false, message = "Error al borrar" });
            }


            _unitOfWork.Factura.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Factura borrada exitosamente";

            return RedirectToAction("Index");

        }

        public IActionResult DetalleFactura(int idFactura)
        {
            var factura = _unitOfWork.Factura.GetID(
                f => f.IdFactura == idFactura,
                includeProperties: "Proyecto,TipoFactura"
            );

            var detalles = _unitOfWork.FacturaProducto.GetAllBYID(
                d => d.IdFactura == idFactura,
                includeProperties: "Producto,Producto.Unidad"
            );

            if (factura == null)
                return NotFound();

            FacturaCompletaVM vm = new()
            {
                Factura = factura,
                Detalles = detalles
            };

            return View(vm);
        }

    }
}
