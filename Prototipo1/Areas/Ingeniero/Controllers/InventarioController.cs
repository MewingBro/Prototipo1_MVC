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
    public class InventarioController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InventarioController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(
    string NombreProducto,
    string CodigoProducto,
    string Familia,
    string Unidad,
     bool? SoloCero)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");
            if (!idProyecto.HasValue)
            {
                TempData["Error"] = "Debe seleccionar un proyecto antes de continuar.";
                return RedirectToAction("Index", "Home");
            }

            var query = _unitOfWork.Inventario.GetAllBYID(
                f => f.IdProyecto == idProyecto,
                includeProperties: "Producto,Proyecto,Producto.Unidad,Producto.Familia"
            );

            if (!string.IsNullOrWhiteSpace(NombreProducto))
                query = query.Where(f => f.Producto.NombreProducto.ToLower().Contains(NombreProducto.ToLower()));

            if (!string.IsNullOrWhiteSpace(CodigoProducto))
                query = query.Where(f => f.Producto.CodigoProducto.ToLower().Contains(CodigoProducto.ToLower()));

            if (!string.IsNullOrWhiteSpace(Familia))
                query = query.Where(f => f.Producto.Familia.NombreFamilia.ToLower().Contains(Familia.ToLower()));

            if (!string.IsNullOrWhiteSpace(Unidad))
                query = query.Where(f => f.Producto.Unidad.NombreUnidad.ToLower().Contains(Unidad.ToLower()));

            if (SoloCero.HasValue && SoloCero.Value)
            {
                query = query.Where(i => i.Existencias == 0);
            }

            var objInventarioLista = query.OrderBy(f => f.Producto.NombreProducto).ToList();


            ViewBag.SoloCero = SoloCero;
            ViewBag.NombreProducto = NombreProducto;
            ViewBag.CodigoProducto = CodigoProducto;
            ViewBag.Familia = Familia;
            ViewBag.Unidad = Unidad;

            return View(objInventarioLista);
        }


        [HttpGet]
        public IActionResult Upsert(int? IdInventario)
        {
            
                // Editar inventario existente
                var inventario = _unitOfWork.Inventario.GetID(
                    i => i.IdInventario == IdInventario,
                    includeProperties: "Producto,Producto.Familia,Producto.Unidad,Proyecto"
                );

                if (inventario == null)
                    return NotFound();

                return View(inventario);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Inventario obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.IdInventario == 0)
                {
                    _unitOfWork.Inventario.Add(obj);
                    TempData["success"] = "Inventario creado exitosamente";
                }
                else
                {
                    _unitOfWork.Inventario.Update(obj);
                    TempData["success"] = "Inventario actualizado exitosamente";
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        [HttpPost]
        public IActionResult Borrar(int id)
        {
            var inventario = _unitOfWork.Inventario.GetID(i => i.IdInventario == id);

            if (inventario == null)
            {
                TempData["Error"] = "El registro de inventario no existe.";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Inventario.Delete(inventario);
            _unitOfWork.Save();

            TempData["Success"] = "Inventario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Reporte()
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            var inventario = _unitOfWork.Inventario.GetAllBYID(
                f => f.IdProyecto == idProyecto,
                includeProperties: "Producto,Producto.Familia,Producto.Unidad,Proyecto"
            ).ToList();

            return View(inventario);
        }


    }
}
