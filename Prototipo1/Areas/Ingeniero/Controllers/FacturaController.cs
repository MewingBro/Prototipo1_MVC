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
    public class FacturaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FacturaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(
    string Tipo,
    int? IdFactura,
    string Aposento,
    string Nivel,
    string Recinto,
    DateTime? Fecha,
    string Comentario,
    string Estado,
    int page = 1,
    int pageSize = 10)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");
            if (!idProyecto.HasValue)
            {
                TempData["Error"] = "Debe seleccionar un proyecto antes de continuar.";
                return RedirectToAction("Index", "Home");
            }

            int? idTipoFactura = null;
            if (!string.IsNullOrEmpty(Tipo))
            {
                if (Tipo.Equals("Entrada", StringComparison.OrdinalIgnoreCase))
                    idTipoFactura = 1;
                else if (Tipo.Equals("Salida", StringComparison.OrdinalIgnoreCase))
                    idTipoFactura = 2;
            }

            // Base query
            var query = _unitOfWork.Factura.GetAllBYID(
                f => f.IdProyecto == idProyecto,
                includeProperties: "TipoFactura,Proyecto,Recinto,Recinto.Aposento.Nivel"
            );

            // Filtro por tipo
            if (idTipoFactura.HasValue)
                query = query.Where(f => f.IdTipoFactura == idTipoFactura.Value);

            // Filtros dinámicos
            if (IdFactura.HasValue)
                query = query.Where(f => f.IdFactura == IdFactura.Value);

            if (!string.IsNullOrWhiteSpace(Comentario))
                query = query.Where(f => f.Comentario != null &&
                                         f.Comentario.ToLower().Contains(Comentario.ToLower()));

            if (Fecha.HasValue)
                query = query.Where(f => f.Fecha.Date == Fecha.Value.Date);

            if (!string.IsNullOrWhiteSpace(Aposento))
                query = query.Where(f => f.Recinto != null &&
                                         f.Recinto.Aposento.NombreAposento.ToLower().Contains(Aposento.ToLower()));

            if (!string.IsNullOrWhiteSpace(Nivel))
                query = query.Where(f => f.Recinto != null &&
                                         f.Recinto.Aposento.Nivel.NombreNivel.ToLower().Contains(Nivel.ToLower()));

            if (!string.IsNullOrWhiteSpace(Recinto))
                query = query.Where(f => f.Recinto != null &&
                                         f.Recinto.NombreRecinto.ToLower().Contains(Recinto.ToLower()));

            // Filtro por estado
            if (!string.IsNullOrWhiteSpace(Estado))
            {
                query = Estado switch
                {
                    "Pendiente" => query.Where(f => f.EstadoFactura == 0),
                    "Cerrada" => query.Where(f => f.EstadoFactura == 1),
                    "Pendiente de aprobación" => query.Where(f => f.EstadoFactura == 2),
                    "Rechazada" => query.Where(f => f.EstadoFactura == 3),
                    _ => query
                };
            }

            // Paginación
            int total = query.Count();
            var objFacturaLista = query
                .OrderByDescending(f => f.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBags
            ViewBag.Tipo = Tipo;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalFacturas = total;
            ViewBag.IdFactura = IdFactura;
            ViewBag.Aposento = Aposento;
            ViewBag.Nivel = Nivel;
            ViewBag.Recinto = Recinto;
            ViewBag.Fecha = Fecha?.ToString("yyyy-MM-dd");
            ViewBag.Comentario = Comentario;
            ViewBag.Estado = Estado;

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
                        int? IdRecinto = FacturaVM.Factura.IdRecinto;
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
                                            new { IdFactura = IdFactura, IdRecinto = IdRecinto } // parámetro
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
