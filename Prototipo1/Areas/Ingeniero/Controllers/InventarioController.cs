using Microsoft.AspNetCore.Mvc;
using Prototipo1.Data;
using Prototipo1.Models;
using System.ComponentModel;
using Prototipo1.Repository;
using Prototipo1.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prototipo1.Models.ViewModels;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    public class InventarioController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InventarioController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");
            List<Inventario> objInventarioLista = 
                _unitOfWork.Inventario.GetAllBYID(f => f.IdProyecto == idProyecto,includeProperties:"Producto,Proyecto").ToList();


            return View(objInventarioLista);
        }

        
    }
}
