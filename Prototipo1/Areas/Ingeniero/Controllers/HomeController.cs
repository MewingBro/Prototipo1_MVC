using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prototipo1.Models;
using Prototipo1.Repository;
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



public IActionResult Index(string searchString, int page = 1)
    {
        
            if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        List<Proyecto> proyectoList = new List<Proyecto>();

        // Admin ve todos los proyectos
        if (User.IsInRole(SD.Role_Admin))
        {
            proyectoList = _unitOfWork.Proyecto.GetAll().ToList();
        }
        // Ingeniero o Bodeguero solo ven proyectos a los cuales tienen acceso
        else if (User.IsInRole(SD.Role_Ingeniero) || User.IsInRole(SD.Role_Bodeguero))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var proyectosUsuario = _unitOfWork.UsuariosProyectos
                    .GetAllBYID(up => up.IdUsuario == userId)
                    .Select(up => up.IdProyecto)
                    .ToList();

                proyectoList = _unitOfWork.Proyecto
                    .GetAllBYID(p => proyectosUsuario.Contains(p.IdProyecto))
                    .ToList();
            }
        }

        // Filtrado por búsqueda
        if (!string.IsNullOrEmpty(searchString))
        {
            proyectoList = proyectoList
                .Where(p => p.NombreProyecto.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Paginación
        const int pageSize = 8;
        int total = proyectoList.Count();
        int totalPages = (int)Math.Ceiling((double)total / pageSize);
        var proyectosPaginados = proyectoList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        //Pasar valores a la vista
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalProyectos = total;
        ViewBag.SearchString = searchString;
        ViewBag.TotalPages = totalPages;

        return View(proyectosPaginados);
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
