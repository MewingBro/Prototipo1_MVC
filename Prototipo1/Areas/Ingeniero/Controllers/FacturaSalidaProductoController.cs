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
    public class FacturaSalidaProductoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FacturaSalidaProductoController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(
    int? IdFactura,
    int? IdRecinto,
    string Producto,
    string EntregadoA,
    string Familia,
    string Unidad)
        {
            if (!IdFactura.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una factura válida.";
                return RedirectToAction("Index", "Factura");
            }

            // Consulta base con includes
            var query = _unitOfWork.FacturaSalidaProducto.GetAllBYID(
                f => f.IdFactura == IdFactura,
                includeProperties: "Factura,Producto,Producto.Familia,Producto.Unidad"
            );

            // Filtros dinámicos
            if (!string.IsNullOrWhiteSpace(Producto))
                query = query.Where(f => f.Producto.NombreProducto.ToLower().Contains(Producto.ToLower()));

            if (!string.IsNullOrWhiteSpace(EntregadoA))
                query = query.Where(f => f.EntregadoA != null && f.EntregadoA.ToLower().Contains(EntregadoA.ToLower()));

            if (!string.IsNullOrWhiteSpace(Familia))
                query = query.Where(f => f.Producto.Familia.NombreFamilia.ToLower().Contains(Familia.ToLower()));

            if (!string.IsNullOrWhiteSpace(Unidad))
                query = query.Where(f => f.Producto.Unidad.NombreUnidad.ToLower().Contains(Unidad.ToLower()));

            // Ejecutar consulta
            var objFacturaSalidaProductoLista = query
                .OrderBy(f => f.Producto.NombreProducto)
                .ToList();

            var recinto = _unitOfWork.Recinto.GetID(f => f.IdRecinto == IdRecinto); 
            if (recinto != null) 
            { 
                if (recinto.EstadoRecinto == 0) 
                { 
                    ViewBag.SinPresupuesto = true; 
                } 
            }

            // ViewBag para mantener filtros
            ViewBag.IdFactura = IdFactura;
            ViewBag.Producto = Producto;
            ViewBag.EntregadoA = EntregadoA;
            ViewBag.Familia = Familia;
            ViewBag.Unidad = Unidad;

            return View(objFacturaSalidaProductoLista);
        }


        [HttpPost]
        public IActionResult TerminarFactura(int? IdFactura)
        {
            // AFECTA EL INVENTARIO Y REDUCE UNIDADES EN PRESUPUESTOS E INVENTARIOS
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
                // PERMITE INICIAR UNA SOLICITUD DE CAMBIO
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
                var detalle = FacturaSalidaProductoVM.FacturaSalidaProducto;

                // Guardar o actualizar registro normalmente
                if (detalle.IdFacturaSalidaProducto == 0)
                {
                    _unitOfWork.FacturaSalidaProducto.Add(detalle);
                    TempData["success"] = "Detalle de Factura agregado exitosamente";
                }
                else
                {
                    _unitOfWork.FacturaSalidaProducto.Update(detalle);
                    TempData["success"] = "Detalle de Factura actualizado exitosamente";
                }

                _unitOfWork.Save();

                // Lógica de validación extra de la media aritmetica

                try
                {
                    // Obtener el producto y su media aritmética (en días)
                    var producto = _unitOfWork.Producto.GetID(p => p.IdProducto == detalle.IdProducto);
                    if (producto == null) throw new Exception("Producto no encontrado");

                    int N = producto.MediaAritmetica;

                    // Obtener la factura y su recinto
                    var factura = _unitOfWork.Factura.GetID(
                        f => f.IdFactura == detalle.IdFactura,
                        includeProperties: "Recinto"
                    );
                    if (factura == null) throw new Exception("Factura no encontrada");

                    int? idRecinto = factura.IdRecinto;

                    // Obtener todas las facturas de salida confirmadas de los últimos N días
                    var fechaCorte = DateTime.Now.AddDays(-N);
                    var facturasRelacionadas = _unitOfWork.Factura.GetAllBYID(
                        f => f.IdTipoFactura == 2 &&
                             f.EstadoFactura == 1 &&
                             f.Fecha >= fechaCorte
                    ).ToList();

                    // Tomamos solo los IDs
                    var facturasRelacionadasIds = facturasRelacionadas.Select(f => f.IdFactura).ToList();

                    // Obtener todas las salidas de ese producto en esas facturas
                    var salidas = _unitOfWork.FacturaSalidaProducto.GetAllBYID(
                        s => s.IdProducto == detalle.IdProducto &&
                             facturasRelacionadasIds.Contains(s.IdFactura)
                    );

                    double totalDisminuido = salidas.Sum(s => s.CantidadDisminuida);

                    // Obtener existencias actuales del recinto-producto
                    var recintoProducto = _unitOfWork.RecintoProducto.GetID(
                        rp => rp.IdRecinto == idRecinto && rp.IdProducto == detalle.IdProducto
                    );

                    if (recintoProducto != null)
                    {
                        double existencias = recintoProducto.ExistenciasActuales;

                        if (totalDisminuido > existencias)
                        {
                            TempData["AlertaInventario"] =
                                $"⚠️ En los últimos {N} días, se han utilizado {totalDisminuido} unidades del producto " +
                                $"{producto.NombreProducto}, superando las existencias actuales ({existencias}). " +
                                $"Continúe con precaución: el inventario podría no alcanzar para cubrir los próximos {N} días.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Evita romper la ejecución si algo falla en la validación
                    Console.WriteLine($"[Advertencia Inventario] {ex.Message}");
                }

                // Redirigir nuevamente al listado
                int idFactura = detalle.IdFactura;
                var facturaRelacionada = _unitOfWork.Factura.GetID(m => m.IdFactura == idFactura);
                int? idRecintoRelacionado = facturaRelacionada?.IdRecinto;

                return RedirectToAction("Index", new { IdFactura = idFactura, IdRecinto = idRecintoRelacionado });
            }

            // Si hay error de validación
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
                return Json(new { success = false, message = "Error al borrar" });
            }


            _unitOfWork.FacturaSalidaProducto.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Detalle borrado exitosamente";

            return RedirectToAction("Index", new { IdFactura = IdFactura });

        }
    }
}
