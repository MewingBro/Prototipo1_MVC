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

        [HttpGet]
        public IActionResult Index(
            string? Completado,
    int? IdRecinto,
    string nombreProducto,
    string nombreRecinto,
    string nombreNivel,
    string nombreAposento,
    bool? SoloCero,
    int page = 1,
    int pageSize = 10)
        {
            ViewBag.Completado = Completado;

            var query = _unitOfWork.RecintoProducto.GetAllBYID(
                f => f.IdRecinto == IdRecinto,
                includeProperties: "Recinto,Recinto.Aposento.Nivel,Producto"
            );

            // 🔍 Filtros dinámicos
            if (!string.IsNullOrWhiteSpace(nombreProducto))
                query = query.Where(rp => rp.Producto.NombreProducto.ToLower().Contains(nombreProducto.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreRecinto))
                query = query.Where(rp => rp.Recinto.NombreRecinto.ToLower().Contains(nombreRecinto.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreNivel))
                query = query.Where(rp => rp.Recinto.Aposento.Nivel.NombreNivel.ToLower().Contains(nombreNivel.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreAposento))
                query = query.Where(rp => rp.Recinto.Aposento.NombreAposento.ToLower().Contains(nombreAposento.ToLower()));

            if (SoloCero.HasValue && SoloCero.Value)
                query = query.Where(rp => rp.ExistenciasActuales == 0);

            // 📊 Paginación
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var objRecintoProductoLista = query
                .OrderBy(rp => rp.IdRecintoProducto)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBag con parámetros
            ViewBag.IdRecinto = IdRecinto;
            ViewBag.NombreProducto = nombreProducto;
            ViewBag.NombreRecinto = nombreRecinto;
            ViewBag.NombreNivel = nombreNivel;
            ViewBag.NombreAposento = nombreAposento;
            ViewBag.SoloCero = SoloCero;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View(objRecintoProductoLista);
        }



        [HttpPost]
        public IActionResult ConfirmarPresupuesto(int? IdRecinto)
        {

            var recinto = _unitOfWork.Recinto.GetID(f => f.IdRecinto == IdRecinto);
            if (recinto != null)
            {
                recinto.EstadoRecinto = 1; // ← cambia el estado
                _unitOfWork.Recinto.Update(recinto);
            }

            _unitOfWork.Save();

            return RedirectToAction("Index", "Recinto");
        }

        public IActionResult Upsert(int? IdRecinto, int? IdRecintoProducto)
        {

            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                return RedirectToAction("SeleccionarProyecto", "Proyecto");
            }

            // 1️ Obtener los productos del inventario del proyecto
            var inventarios = _unitOfWork.Inventario
                .GetAllBYID(i => i.IdProyecto == idProyecto)
                .ToList();

            // 2️ Extraer sus IdProducto
            var idsProductosInventario = inventarios
                .Select(i => i.IdProducto)
                .ToList();

            // 3️ Obtener todos los productos y luego filtrar en memoria
            var productos = _unitOfWork.Producto
                .GetAll()
                .Where(p => idsProductosInventario.Contains(p.IdProducto))
                .ToList();

            // 4️ Armar la lista para el dropdown
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

        [HttpGet]
        public IActionResult Detalle(
    string nombreNivel,
    string nombreAposento,
    string nombreRecinto,
    string nombreProducto,
    string Estado,
    bool? SoloCero,
    int page = 1,
    int pageSize = 10)
        {
            var query = _unitOfWork.RecintoProducto.GetAll(
                includeProperties: "Recinto,Recinto.Aposento.Nivel,Producto"
            );

            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            query = query.Where(n => n.Recinto.Aposento.Nivel.IdProyecto == idProyecto);


            // 🔍 Filtros dinámicos
            if (!string.IsNullOrWhiteSpace(nombreNivel))
                query = query.Where(rp => rp.Recinto.Aposento.Nivel.NombreNivel.ToLower().Contains(nombreNivel.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreAposento))
                query = query.Where(rp => rp.Recinto.Aposento.NombreAposento.ToLower().Contains(nombreAposento.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreRecinto))
                query = query.Where(rp => rp.Recinto.NombreRecinto.ToLower().Contains(nombreRecinto.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreProducto))
                query = query.Where(rp => rp.Producto.NombreProducto.ToLower().Contains(nombreProducto.ToLower()));

            if (SoloCero.HasValue && SoloCero.Value)
                query = query.Where(rp => rp.ExistenciasActuales == 0);

            if (!string.IsNullOrWhiteSpace(Estado))
            {
                query = Estado switch
                {
                    "Pendiente" => query.Where(f => f.Recinto.EstadoRecinto == 0),
                    "Confirmado" => query.Where(f => f.Recinto.EstadoRecinto == 1),
                   
                    _ => query
                };
            }

            // 📊 Paginación
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var lista = query
                .OrderBy(rp => rp.IdRecintoProducto)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBags
            ViewBag.NombreNivel = nombreNivel;
            ViewBag.NombreAposento = nombreAposento;
            ViewBag.NombreRecinto = nombreRecinto;
            ViewBag.NombreProducto = nombreProducto;
            ViewBag.SoloCero = SoloCero;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Estado = Estado;

            return View(lista);
        }

        [HttpGet]
        public IActionResult Reporte(
    string nombreNivel,
    string nombreAposento,
    string nombreRecinto,
    string nombreProducto,
    string Estado,
    bool? SoloCero)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");
            var Proyecto = _unitOfWork.Proyecto.GetID(m => m.IdProyecto == idProyecto);
            ViewBag.Proyecto = Proyecto.NombreProyecto;

            var query = _unitOfWork.RecintoProducto.GetAll(
                includeProperties: "Recinto,Recinto.Aposento.Nivel,Producto"
            );

            // Filtrar solo por proyecto
            query = query.Where(rp => rp.Recinto.Aposento.Nivel.IdProyecto == idProyecto);

            // 🔍 Aplicar los filtros que vinieron del view Detalle
            if (!string.IsNullOrWhiteSpace(nombreNivel))
                query = query.Where(rp => rp.Recinto.Aposento.Nivel.NombreNivel.ToLower().Contains(nombreNivel.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreAposento))
                query = query.Where(rp => rp.Recinto.Aposento.NombreAposento.ToLower().Contains(nombreAposento.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreRecinto))
                query = query.Where(rp => rp.Recinto.NombreRecinto.ToLower().Contains(nombreRecinto.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreProducto))
                query = query.Where(rp => rp.Producto.NombreProducto.ToLower().Contains(nombreProducto.ToLower()));

            if (SoloCero.HasValue && SoloCero.Value)
                query = query.Where(rp => rp.ExistenciasActuales == 0);

            if (!string.IsNullOrWhiteSpace(Estado))
            {
                query = Estado switch
                {
                    "Pendiente" => query.Where(rp => rp.Recinto.EstadoRecinto == 0),
                    "Confirmado" => query.Where(rp => rp.Recinto.EstadoRecinto == 1),
                    _ => query
                };
            }

            var listaFiltrada = query.OrderBy(rp => rp.IdRecintoProducto).ToList();

            return View(listaFiltrada); // Pasamos la lista filtrada al view
        }

        [HttpGet]
        public IActionResult PorcentajeExistencias(
    string nombreNivel,
    string nombreAposento,
    string nombreRecinto,
    int page = 1)
        {
            int pageSize = 10;

            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            var query = _unitOfWork.RecintoProducto.GetAll(
                includeProperties: "Recinto,Recinto.Aposento.Nivel,Producto"
            );

            // 🔹 Filtrar por proyecto
            if (idProyecto.HasValue)
                query = query.Where(rp => rp.Recinto.Aposento.Nivel.IdProyecto == idProyecto);

            // 🔍 Filtros dinámicos
            if (!string.IsNullOrWhiteSpace(nombreNivel))
                query = query.Where(rp => rp.Recinto.Aposento.Nivel.NombreNivel.ToLower().Contains(nombreNivel.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreAposento))
                query = query.Where(rp => rp.Recinto.Aposento.NombreAposento.ToLower().Contains(nombreAposento.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreRecinto))
                query = query.Where(rp => rp.Recinto.NombreRecinto.ToLower().Contains(nombreRecinto.ToLower()));

            // 📊 Cálculo de porcentajes totales por recinto
            var totalesPorRecinto = query
                .GroupBy(rp => rp.Recinto.NombreRecinto)
                .Select(g => new
                {
                    Recinto = g.Key,
                    TotalPresupuesto = g.Sum(x => x.Presupuesto),
                    TotalExistencias = g.Sum(x => x.ExistenciasActuales)
                })
                .ToDictionary(
                    x => x.Recinto,
                    x => x.TotalPresupuesto > 0
                        ? 100-((x.TotalExistencias / (double)x.TotalPresupuesto) * 100)
                        : 0
                );

            // 📄 Total de registros antes de paginar
            int totalRegistros = query.Count();
            int totalPages = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            // 📦 Aplicar paginación y proyección
            var lista = query
                .OrderBy(rp => rp.Recinto.Aposento.Nivel.NombreNivel)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(rp => new
                {
                    Nivel = rp.Recinto.Aposento.Nivel.NombreNivel,
                    Aposento = rp.Recinto.Aposento.NombreAposento,
                    Recinto = rp.Recinto.NombreRecinto,
                    Producto = rp.Producto.NombreProducto,
                    Presupuesto = rp.Presupuesto,
                    Existencias = rp.ExistenciasActuales,
                    ExistenciasUsadas = rp.Presupuesto - rp.ExistenciasActuales,
                    Porcentaje = rp.Presupuesto > 0
                        ? 100-((rp.ExistenciasActuales / (double)rp.Presupuesto) * 100)
                        : 0,
                    PorcentajeTotalRecinto = totalesPorRecinto.ContainsKey(rp.Recinto.NombreRecinto)
                        ? totalesPorRecinto[rp.Recinto.NombreRecinto]
                        : 0
                })
                .ToList();

            // 📋 Guardar filtros y datos de paginación
            ViewBag.NombreNivel = nombreNivel;
            ViewBag.NombreAposento = nombreAposento;
            ViewBag.NombreRecinto = nombreRecinto;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View(lista);
        }


        [HttpGet]
        public IActionResult ReportePorcentajeExistencias(
    string nombreNivel,
    string nombreAposento,
    string nombreRecinto)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            var Proyecto = _unitOfWork.Proyecto.GetID(m => m.IdProyecto == idProyecto);

            string nombreProyecto = Proyecto.NombreProyecto;

            var query = _unitOfWork.RecintoProducto.GetAll(
                includeProperties: "Recinto,Recinto.Aposento.Nivel,Producto"
            );

            var totalesPorRecinto = query
                .GroupBy(rp => rp.Recinto.NombreRecinto)
                .Select(g => new
                {
                    Recinto = g.Key,
                    TotalPresupuesto = g.Sum(x => x.Presupuesto),
                    TotalExistencias = g.Sum(x => x.ExistenciasActuales)
                })
                .ToDictionary(
                    x => x.Recinto,
                    x => x.TotalPresupuesto > 0
                        ? 100-((x.TotalExistencias / (double)x.TotalPresupuesto) * 100)
                        : 0
                );

            if (idProyecto.HasValue)
                query = query.Where(rp => rp.Recinto.Aposento.Nivel.IdProyecto == idProyecto);

            // Filtros
            if (!string.IsNullOrWhiteSpace(nombreNivel))
                query = query.Where(rp => rp.Recinto.Aposento.Nivel.NombreNivel.ToLower().Contains(nombreNivel.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreAposento))
                query = query.Where(rp => rp.Recinto.Aposento.NombreAposento.ToLower().Contains(nombreAposento.ToLower()));

            if (!string.IsNullOrWhiteSpace(nombreRecinto))
                query = query.Where(rp => rp.Recinto.NombreRecinto.ToLower().Contains(nombreRecinto.ToLower()));

            // Generar lista
            var lista = query
                .OrderBy(rp => rp.Recinto.Aposento.Nivel.NombreNivel)
                .ToList()
                .Select(rp => new
                {
                    Nivel = rp.Recinto.Aposento.Nivel.NombreNivel,
                    Aposento = rp.Recinto.Aposento.NombreAposento,
                    Recinto = rp.Recinto.NombreRecinto,
                    Producto = rp.Producto.NombreProducto,
                    Presupuesto = rp.Presupuesto,
                    Existencias = rp.ExistenciasActuales,
                    ExistenciasUsadas = rp.Presupuesto - rp.ExistenciasActuales,
                    Porcentaje = rp.Presupuesto > 0
                        ? 100-((rp.ExistenciasActuales / (double)rp.Presupuesto) * 100)
                        : 0,
                    PorcentajeTotalRecinto = totalesPorRecinto.ContainsKey(rp.Recinto.NombreRecinto)
                        ? totalesPorRecinto[rp.Recinto.NombreRecinto]
                        : 0
                })
                .ToList();

            ViewBag.FechaGeneracion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewBag.Proyecto = nombreProyecto;

            return View("ReportePorcentajeExistencias", lista);
        }


    }
}
