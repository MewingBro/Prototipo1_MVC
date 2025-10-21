using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prototipo1.Models;
using Prototipo1.Repository.IRepository;
using System.Diagnostics;

namespace Prototipo1.Areas.Ingeniero.Controllers
{
    [Area("Ingeniero")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            IEnumerable<Proyecto> proyectoList = _unitOfWork.Proyecto.GetAll();
            return View(proyectoList);
        }

        [HttpPost]
        public IActionResult SeleccionarProyecto(int IdProyecto, string NombreProyecto)
        {
            // Guardar en la sesión
            HttpContext.Session.SetInt32("IdProyecto", IdProyecto);
            HttpContext.Session.SetString("NombreProyecto", NombreProyecto);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
