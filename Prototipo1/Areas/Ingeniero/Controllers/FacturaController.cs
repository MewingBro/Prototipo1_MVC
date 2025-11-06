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
    string? Impresion,
    string Aposento,
    string Nivel,
    string Recinto,
    DateTime? Fecha,
    string Comentario,
    string Estado,
    int page = 1,
    int pageSize = 10)
    
        {
            ViewBag.Impresion = Impresion;
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

        public IActionResult DetalleSalida(int idFactura)
        {
            // Obtener la factura junto con sus relaciones
            var factura = _unitOfWork.Factura.GetID(
                f => f.IdFactura == idFactura,
                includeProperties: "Proyecto,TipoFactura,Recinto,Recinto.Aposento,Recinto.Aposento.Nivel"
            );

            if (factura == null)
                return NotFound();

            // Obtener los productos asociados a la factura de salida
            var detalles = _unitOfWork.FacturaSalidaProducto.GetAllBYID(
                d => d.IdFactura == idFactura,
                includeProperties: "Producto,Producto.Unidad,Producto.Familia,Factura"
            );

            // Construir el ViewModel
            var vm = new FacturaCompletaSalidaVM
            {
                Factura = factura,
                Detalles = detalles
            };

            return View(vm);
        }

        public IActionResult Kardex(int? idProducto, int? idRecinto)
        {
            KardexVM vm = new();

            // Proyecto actual desde sesión
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");
            if (idProyecto == null)
            {
                TempData["Error"] = "Debe seleccionar un proyecto antes de generar el reporte Kardex.";
                return RedirectToAction("Index", "Proyecto");
            }

            // 🔹 Cargar lista de recintos del proyecto actual
            vm.Recintos = _unitOfWork.Recinto
                .GetAllBYID(r => r.Aposento.Nivel.IdProyecto == idProyecto, includeProperties: "Aposento.Nivel")
                .OrderBy(r => r.NombreRecinto)
                .ToList();

            // Si no hay producto ni recinto seleccionado, devolver vista vacía
            if (idProducto == null && idRecinto == null)
                return View(vm);

            // 🔹 Obtener el producto (si se seleccionó)
            if (idProducto != null)
            {
                var producto = _unitOfWork.Producto.GetID(
                    p => p.IdProducto == idProducto,
                    includeProperties: "Familia,Unidad"
                );

                if (producto == null)
                    return NotFound();

                vm.Producto = producto;
            }

            // 🔹 Entradas (filtrar por recinto si se indicó)
            var entradas = _unitOfWork.FacturaProducto.GetAllBYID(
                f =>
                    (idProducto == null || f.IdProducto == idProducto) &&
                    f.Factura.IdProyecto == idProyecto &&
                    (idRecinto == null || f.Factura.IdRecinto == idRecinto),
                includeProperties: "Factura,Factura.Recinto,Factura.Recinto.Aposento.Nivel"
            ).Select(e => new KardexItemVM
            {
                FechaFactura = e.Factura.Fecha,
                IdFactura = e.IdFactura,
                TipoFactura = "Entrada",
                CantidadAumentada = e.CantidadAumentada,
                CantidadDisminuida = 0,
                Comentario = e.Factura.Comentario,
                Recinto = e.Factura.Recinto?.NombreRecinto ?? "-",
                Nivel = e.Factura.Recinto?.Aposento?.Nivel?.NombreNivel ?? "-",
                Aposento = e.Factura.Recinto?.Aposento?.NombreAposento ?? "-"
            });

            // 🔹 Salidas
            var salidas = _unitOfWork.FacturaSalidaProducto.GetAllBYID(
                f =>
                    (idProducto == null || f.IdProducto == idProducto) &&
                    f.Factura.IdProyecto == idProyecto &&
                    (idRecinto == null || f.Factura.IdRecinto == idRecinto),
                includeProperties: "Factura,Factura.Recinto,Factura.Recinto.Aposento.Nivel"
            ).Select(s => new KardexItemVM
            {
                FechaFactura = s.Factura.Fecha,
                IdFactura = s.IdFactura,
                TipoFactura = "Salida",
                CantidadAumentada = 0,
                CantidadDisminuida = s.CantidadDisminuida,
                Comentario = s.Factura.Comentario,
                Recinto = s.Factura.Recinto?.NombreRecinto ?? "-",
                Nivel = s.Factura.Recinto?.Aposento?.Nivel?.NombreNivel ?? "-",
                Aposento = s.Factura.Recinto?.Aposento?.NombreAposento ?? "-"
            });

            // 🔹 Combinar y ordenar cronológicamente
            var movimientos = entradas.Concat(salidas)
                .OrderBy(m => m.FechaFactura)
                .ToList();

            // 🔹 Calcular saldo acumulado
            int saldo = 0;
            foreach (var m in movimientos)
            {
                saldo += m.CantidadAumentada;
                saldo -= m.CantidadDisminuida;
                m.Saldo = saldo;
            }

            vm.Movimientos = movimientos;

            // Guardar recinto seleccionado
            if (idRecinto != null)
                vm.RecintoSeleccionadoId = idRecinto.Value;

            return View(vm);
        }



        public IActionResult ImprimirKardex(int idProducto, int? idRecinto)
        {
            // Obtenemos el proyecto actual
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");
            if (idProyecto == null)
            {
                TempData["Error"] = "Debe seleccionar un proyecto antes de imprimir el Kardex.";
                return RedirectToAction("Index", "Proyecto");
            }

            var proyecto = _unitOfWork.Proyecto.GetID(m => m.IdProyecto == idProyecto);
            ViewBag.Proyecto = proyecto?.NombreProyecto ?? "Proyecto desconocido";

            KardexVM vm = new();

            // Obtenemos el producto con relaciones
            var producto = _unitOfWork.Producto.GetID(
                p => p.IdProducto == idProducto,
                includeProperties: "Familia,Unidad"
            );
            if (producto == null)
                return NotFound();

            vm.Producto = producto;

            // Facturas de entrada
            var entradas = _unitOfWork.FacturaProducto.GetAllBYID(
                f => f.IdProducto == idProducto &&
                     f.Factura.IdProyecto == idProyecto &&
                     (idRecinto == null || f.Factura.IdRecinto == idRecinto),
                includeProperties: "Factura,Factura.Recinto,Factura.Recinto.Aposento.Nivel"
            ).Select(e => new KardexItemVM
            {
                FechaFactura = e.Factura.Fecha,
                IdFactura = e.IdFactura,
                TipoFactura = "Entrada",
                CantidadAumentada = e.CantidadAumentada,
                CantidadDisminuida = 0,
                Comentario = e.Factura.Comentario,
                Recinto = e.Factura.Recinto?.NombreRecinto ?? "-",
                Nivel = e.Factura.Recinto?.Aposento?.Nivel?.NombreNivel ?? "-",
                Aposento = e.Factura.Recinto?.Aposento?.NombreAposento ?? "-"
            });

            // Facturas de salida
            var salidas = _unitOfWork.FacturaSalidaProducto.GetAllBYID(
                f => f.IdProducto == idProducto &&
                     f.Factura.IdProyecto == idProyecto &&
                     (idRecinto == null || f.Factura.IdRecinto == idRecinto),
                includeProperties: "Factura,Factura.Recinto,Factura.Recinto.Aposento.Nivel"
            ).Select(s => new KardexItemVM
            {
                FechaFactura = s.Factura.Fecha,
                IdFactura = s.IdFactura,
                TipoFactura = "Salida",
                CantidadAumentada = 0,
                CantidadDisminuida = s.CantidadDisminuida,
                Comentario = s.Factura.Comentario,
                Recinto = s.Factura.Recinto?.NombreRecinto ?? "-",
                Nivel = s.Factura.Recinto?.Aposento?.Nivel?.NombreNivel ?? "-",
                Aposento = s.Factura.Recinto?.Aposento?.NombreAposento ?? "-"
            });

            // Combinar y ordenar cronológicamente
            var movimientos = entradas.Concat(salidas)
                .OrderBy(m => m.FechaFactura)
                .ToList();

            // Calcular saldo acumulado
            int saldo = 0;
            foreach (var m in movimientos)
            {
                saldo += m.CantidadAumentada;
                saldo -= m.CantidadDisminuida;
                m.Saldo = saldo;
            }

            vm.Movimientos = movimientos;

            // Guardamos el nombre del recinto (si aplica)
            if (idRecinto != null)
            {
                var recinto = _unitOfWork.Recinto.GetID(r => r.IdRecinto == idRecinto);
                ViewBag.Recinto = recinto?.NombreRecinto ?? "Desconocido";
            }

            return View("KardexImprimir", vm);
        }




    }
}
