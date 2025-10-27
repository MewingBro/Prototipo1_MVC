using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prototipo1.Models;
using Prototipo1.Repository.IRepository;
using Prototipo1.Utility;
using System.Diagnostics;
using System.Security.Claims;

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

            IEnumerable<Proyecto> proyectoList = null;

            if (User.IsInRole(SD.Role_Admin))
            {
                proyectoList = _unitOfWork.Proyecto.GetAll();
            } else if (User.IsInRole(SD.Role_Ingeniero) || User.IsInRole(SD.Role_Bodeguero))
            {
                // 1️⃣ Obtener el Id del usuario logueado
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // 2️⃣ Obtener los Ids de proyectos que le pertenecen
                var proyectosUsuario = _unitOfWork.UsuariosProyectos
                    .GetAllBYID(up => up.IdUsuario == userId)
                    .Select(up => up.IdProyecto)
                    .ToList();

                // 3️⃣ Obtener solo esos proyectos
                proyectoList = _unitOfWork.Proyecto
                    .GetAllBYID(p => proyectosUsuario.Contains(p.IdProyecto));
            }
            else
            {
                proyectoList = Enumerable.Empty<Proyecto>();
            }


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
