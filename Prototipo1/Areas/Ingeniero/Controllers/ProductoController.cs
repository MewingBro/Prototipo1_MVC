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


        public IActionResult Index()
        {
            List<Producto> objProductoLista = _unitOfWork.Producto.GetAll(includeProperties:"Familia,Unidad").ToList();


            return View(objProductoLista);
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
            if (IdProducto == null || IdProducto ==0)
            {
                //crear
                return View(ProductoVM);
            }
            else
            {
                //update
                ProductoVM.Producto = _unitOfWork.Producto.GetID(u=>u.IdProducto == IdProducto);
                return View(ProductoVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductoVM ProductoVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null) { 
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

                    

                    using (var fileStream = new FileStream(Path.Combine(ProductoPATH, fileName), FileMode.Create)) { 
                        file.CopyTo(fileStream);
                    }
                    ProductoVM.Producto.ImageURL = @"\Images\Producto\" + fileName;
                }

                if (ProductoVM.Producto.IdProducto == 0)
                {
                    _unitOfWork.Producto.Add(ProductoVM.Producto);
                } else
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
        /*
        public IActionResult Editar(int? IdProducto)
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
        */

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
                return Json( new { success = false, message = "Error al borrar" });
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
    }
}
