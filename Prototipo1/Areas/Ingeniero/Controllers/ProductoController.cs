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
    public class ProductoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductoController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index(string searchString, int? idFamilia, int? idUnidad, int page = 1)
        {
            const int pageSize = 10;

            // Incluye relaciones
            var productos = _unitOfWork.Producto.GetAll(includeProperties: "Familia,Unidad").AsQueryable();

            // 🔍 Filtro por texto
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                productos = productos.Where(p =>
                    p.NombreProducto.ToLower().Contains(searchLower) ||
                    p.CodigoProducto.ToLower().Contains(searchLower));
            }

            // 🧩 Filtro por familia
            if (idFamilia.HasValue && idFamilia.Value > 0)
            {
                productos = productos.Where(p => p.IdFamilia == idFamilia.Value);
            }

            // ⚙️ Filtro por unidad
            if (idUnidad.HasValue && idUnidad.Value > 0)
            {
                productos = productos.Where(p => p.IdUnidad == idUnidad.Value);
            }

            // 📄 Total y paginación
            var totalProductos = productos.Count();
            var productosPaginados = productos
                .OrderBy(p => p.IdProducto)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 🔹 Cargar listas para los dropdowns
            ViewBag.FamiliaList = _unitOfWork.Familia.GetAll()
                .Select(f => new SelectListItem { Text = f.NombreFamilia, Value = f.IdFamilia.ToString() });

            ViewBag.UnidadList = _unitOfWork.Unidad.GetAll()
                .Select(u => new SelectListItem { Text = u.NombreUnidad, Value = u.IdUnidad.ToString() });

            // 🔹 Variables para la vista
            ViewBag.SearchString = searchString;
            ViewBag.IdFamilia = idFamilia;
            ViewBag.IdUnidad = idUnidad;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalProductos = totalProductos;

            return View(productosPaginados);
        }


        public IActionResult Upsert(int? IdProducto)
        {
            IEnumerable<SelectListItem> FamiliaList = _unitOfWork.Familia.GetAll().Select(u => new SelectListItem
            {
                Text = u.NombreFamilia,
                Value = u.IdFamilia.ToString()
            }
            );
            IEnumerable<SelectListItem> UnidadList = _unitOfWork.Unidad.GetAll().Select(u => new SelectListItem
            {
                Text = u.NombreUnidad,
                Value = u.IdUnidad.ToString()
            }
            );

            //ViewBag.FamiliaList = FamiliaList;
            //ViewBag.UnidadList = UnidadList;
            ProductoVM ProductoVM = new()
            {
                FamiliaList = FamiliaList,
                UnidadList = UnidadList,
                Producto = new Producto()

            };
            if (IdProducto == null || IdProducto == 0)
            {
                //crear
                return View(ProductoVM);
            }
            else
            {
                //update
                ProductoVM.Producto = _unitOfWork.Producto.GetID(u => u.IdProducto == IdProducto);
                return View(ProductoVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductoVM ProductoVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProductoPATH = Path.Combine(wwwRootPath, @"Images\Producto");

                    if (!string.IsNullOrEmpty(ProductoVM.Producto.ImageURL))
                    {
                        //borra la imagen anterior
                        var oldImagePath = Path.Combine(wwwRootPath, ProductoVM.Producto.ImageURL.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }



                    using (var fileStream = new FileStream(Path.Combine(ProductoPATH, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    ProductoVM.Producto.ImageURL = @"\Images\Producto\" + fileName;
                }

                if (ProductoVM.Producto.IdProducto == 0)
                {
                    _unitOfWork.Producto.Add(ProductoVM.Producto);
                }
                else
                {
                    _unitOfWork.Producto.Update(ProductoVM.Producto);
                }

                _unitOfWork.Save();
                TempData["success"] = "Producto agregado exitosamente";
                return RedirectToAction("Index");
            }
            else
            {
                ProductoVM.FamiliaList = _unitOfWork.Familia.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreFamilia,
                    Value = u.IdFamilia.ToString()
                });
                ProductoVM.UnidadList = _unitOfWork.Unidad.GetAll().Select(u => new SelectListItem
                {
                    Text = u.NombreUnidad,
                    Value = u.IdUnidad.ToString()
                });
                return View(ProductoVM);
            }



        }


        [HttpPost]
        public IActionResult Editar(Producto obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Producto.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Producto actualizada exitosamente";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Borrar(int? IdProducto)
        {
            if (IdProducto == null || IdProducto == 0)
            {
                return NotFound();
            }

            Producto? Producto = _unitOfWork.Producto.GetID(u => u.IdProducto == IdProducto);
            if (Producto == null)
            {
                return NotFound();
            }
            return View(Producto);
        }

        [HttpPost, ActionName("Borrar")]
        public IActionResult BorrarPOST(int? IdProducto)
        {
            Producto? obj = _unitOfWork.Producto.GetID(u => u.IdProducto == IdProducto);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error al borrar" });
            }

            var imagenBorrar = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageURL.TrimStart('\\'));

            if (System.IO.File.Exists(imagenBorrar))
            {
                System.IO.File.Delete(imagenBorrar);
            }
            _unitOfWork.Producto.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Producto borrado exitosamente";

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult BuscarProductos(string filtro)
        {
            var productos = _unitOfWork.Producto
                .GetAllBYID(p => p.CodigoProducto.Contains(filtro) || p.NombreProducto.Contains(filtro),
                    includeProperties: "Familia,Unidad")
                .Select(p => new {
                    p.IdProducto,
                    p.CodigoProducto,
                    p.NombreProducto,
                    Familia = p.Familia.NombreFamilia,
                    Unidad = p.Unidad.NombreUnidad,
                    p.MediaAritmetica,
                    p.Descripcion
                }).ToList();

            return Json(productos);
        }

        [HttpGet]
        public IActionResult BuscarProductosPresupuestados(string filtro, int idFactura)
        {
            // Obtener la factura con su recinto
            var factura = _unitOfWork.Factura.GetID(
                f => f.IdFactura == idFactura,
                includeProperties: "Recinto"
            );

            if (factura == null || factura.IdRecinto == null)
                return Json(new List<object>()); // sin resultados

            // Obtener solo productos presupuestados de ese recinto
            var productos = _unitOfWork.RecintoProducto
                .GetAllBYID(
                    rp => rp.IdRecinto == factura.IdRecinto &&
                          (rp.Producto.CodigoProducto.Contains(filtro) || rp.Producto.NombreProducto.Contains(filtro)),
                    includeProperties: "Producto.Familia,Producto.Unidad"
                )
                .Select(rp => new
                {
                    idProducto = rp.Producto.IdProducto,
                    codigoProducto = rp.Producto.CodigoProducto,
                    nombreProducto = rp.Producto.NombreProducto,
                    familia = rp.Producto.Familia.NombreFamilia,
                    unidad = rp.Producto.Unidad.NombreUnidad,
                    mediaArtimetica = rp.Producto.MediaAritmetica,
                    descripcion = rp.Producto.Descripcion
                })
                .Distinct()
                .ToList();

            return Json(productos);
        }

        [HttpGet]
        public IActionResult BuscarProductosInventario(string filtro)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                return Json(new { error = "No hay proyecto seleccionado." });
            }

            // 1️⃣ Obtener los productos del inventario del proyecto actual
            var inventarios = _unitOfWork.Inventario
                .GetAllBYID(i => i.IdProyecto == idProyecto)
                .ToList();

            var idsProductosInventario = inventarios
                .Select(i => i.IdProducto)
                .ToList();

            // 2️⃣ Buscar solo productos que estén en ese inventario
            var productos = _unitOfWork.Producto
                .GetAllBYID(
                    p => idsProductosInventario.Contains(p.IdProducto) &&
                         (p.CodigoProducto.Contains(filtro) || p.NombreProducto.Contains(filtro)),
                    includeProperties: "Familia,Unidad"
                )
                .Select(p => new {
                    idProducto = p.IdProducto,
                    codigoProducto = p.CodigoProducto,
                    nombreProducto = p.NombreProducto,
                    familia = p.Familia.NombreFamilia,
                    unidad = p.Unidad.NombreUnidad,
                    mediaAritmetica = p.MediaAritmetica,
                    descripcion = p.Descripcion
                })
                .ToList();

            return Json(productos);
        }

        [HttpGet]
        public IActionResult ObtenerInventarioProducto(int idProducto)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                return Json(new { existencias = 0 });
            }

            var inventario = _unitOfWork.Inventario
                .GetID(i => i.IdProyecto == idProyecto && i.IdProducto == idProducto);

            if (inventario == null)
            {
                return Json(new { existencias = 0 });
            }

            return Json(new { existencias = inventario.Existencias });
        }

    }
}
