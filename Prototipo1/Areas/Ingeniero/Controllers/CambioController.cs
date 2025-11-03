using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class CambioController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        public CambioController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public IActionResult Index(int? page, int? IdFactura, string email, string comentario, DateTime? fecha, string estado)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");
            if (!idProyecto.HasValue)
            {
                TempData["Error"] = "Debe seleccionar un proyecto antes de continuar.";
                return RedirectToAction("Index", "Home");
            }

            var userId = _userManager.GetUserId(User);
            var roles = _userManager.GetRolesAsync(_userManager.GetUserAsync(User).Result).Result;
            bool esIngeniero = roles.Contains("Ingeniero");

            IEnumerable<Cambio> cambiosQuery;

            if (esIngeniero)
            {
                // Solo las solicitudes del ingeniero logueado
                cambiosQuery = _unitOfWork.Cambio.GetAllBYID(
                    c => c.Factura.Proyecto.IdProyecto == idProyecto.Value
                         && c.IdUsuario == userId,
                    includeProperties: "Factura,Factura.Proyecto,Usuario"
                );
            }
            else
            {
                // Administrador o Bodeguero → todas las solicitudes del proyecto
                cambiosQuery = _unitOfWork.Cambio.GetAllBYID(
                    c => c.Factura.Proyecto.IdProyecto == idProyecto.Value,
                    includeProperties: "Factura,Factura.Proyecto,Usuario"
                );
            }

            // FILTROS
            if (IdFactura.HasValue)
                cambiosQuery = cambiosQuery.Where(c => c.IdFactura == IdFactura.Value);

            if (!string.IsNullOrWhiteSpace(email))
                cambiosQuery = cambiosQuery.Where(c => c.Usuario.Email.ToLower().Contains(email.ToLower()));

            if (!string.IsNullOrWhiteSpace(comentario))
                cambiosQuery = cambiosQuery.Where(c => c.Factura.Comentario.ToLower().Contains(comentario.ToLower()));

            if (fecha.HasValue)
                cambiosQuery = cambiosQuery.Where(c => c.Factura.Fecha.Date == fecha.Value.Date);

            if (!string.IsNullOrWhiteSpace(estado))
                cambiosQuery = cambiosQuery.Where(c => c.Estado.ToLower() == estado.ToLower());

            // Paginación
            int total = cambiosQuery.Count();
            var cambios = cambiosQuery
                .OrderBy(c => c.IdCambio)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCambios = total;
            ViewBag.IdFactura = IdFactura;
            ViewBag.Email = email;
            ViewBag.Comentario = comentario;
            ViewBag.Fecha = fecha?.ToString("yyyy-MM-dd");
            ViewBag.Estado = estado;

            return View(cambios);
        }



        [HttpPost]
        [Authorize] // Asegura que solo usuarios logueados puedan crear solicitudes
        public IActionResult Create(int IdFactura)
        {
            // 🔹 Obtener el usuario actual (Identity)
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Debe iniciar sesión para crear una solicitud de cambio.";
                return RedirectToAction("Login", "Account");
            }

            // 🔹 Crear la solicitud
            Cambio nuevaSolicitud = new Cambio
            {
                IdFactura = IdFactura,
                Estado = "Pendiente",
                IdUsuario = userId,
                FechaSolicitud = DateTime.Now 
            };

            // Guardar en base de datos
            _unitOfWork.Cambio.Add(nuevaSolicitud);
            _unitOfWork.Save();

            // Cambiar estado de la factura
            var factura = _unitOfWork.Factura.GetID(f => f.IdFactura == IdFactura);
            if (factura != null)
            {
                factura.EstadoFactura = 2; // Pendiente
                _unitOfWork.Factura.Update(factura);
            }
            _unitOfWork.Save();

            TempData["Success"] = "Solicitud de cambio creada correctamente.";
            return RedirectToAction("Index");
        }


        public IActionResult Detalle(int IdCambio)
        {
            // Obtener la solicitud
            var cambio = _unitOfWork.Cambio.GetID(
                c => c.IdCambio == IdCambio,
                includeProperties: "Factura,Factura.Proyecto,Usuario"
            );

            if (cambio == null)
                return NotFound();

            int idFactura = cambio.IdFactura;
            int idProyecto = cambio.Factura.IdProyecto;
            int? idRecinto = cambio.Factura.IdRecinto;

            // Obtener productos asociados a la factura
            var productos = _unitOfWork.FacturaSalidaProducto.GetAllBYID(
                f => f.IdFactura == idFactura,
                includeProperties: "Producto"
            ).ToList();

            // 🔹 Construir una lista de objetos para mostrar los datos combinados
            var detalleVM = productos.Select(p =>
            {
                // Buscar existencias actuales en RecintoProducto
                var recintoProd = _unitOfWork.RecintoProducto.GetID(
                    r => r.IdRecinto == idRecinto && r.IdProducto == p.IdProducto
                );

                // Buscar inventario general del proyecto
                var inventario = _unitOfWork.Inventario.GetID(
                    i => i.IdProducto == p.IdProducto && i.IdProyecto == idProyecto
                );

                return new CambioDetalleVM
                {
                    NombreProducto = p.Producto.NombreProducto,
                    CantidadDisminuida = p.CantidadDisminuida,
                    ExistenciasActuales = (int)(recintoProd?.ExistenciasActuales ?? 0),
                    InventarioProyecto = inventario?.Existencias ?? 0,
                    SeExcede = (recintoProd != null && p.CantidadDisminuida > recintoProd.ExistenciasActuales)
                };
            }).ToList();

            ViewBag.IdCambio = IdCambio;
            ViewBag.Estado = cambio.Estado;

            return View(detalleVM);
        }

        [HttpPost]
        public IActionResult Aprobar(int IdCambio)
        {
            var cambio = _unitOfWork.Cambio.GetID(
                c => c.IdCambio == IdCambio,
                includeProperties: "Factura,Factura.Proyecto"
            );

            if (cambio == null)
                return NotFound();

            int idFactura = cambio.IdFactura;
            int idProyecto = cambio.Factura.IdProyecto;
            int? idRecinto = cambio.Factura.IdRecinto;

            // Obtener los productos asociados a la factura
            var productos = _unitOfWork.FacturaSalidaProducto.GetAllBYID(
                f => f.IdFactura == idFactura,
                includeProperties: "Producto"
            ).ToList();

            foreach (var p in productos)
            {
                // Buscar el registro de RecintoProducto
                var recintoProducto = _unitOfWork.RecintoProducto.GetID(
                    r => r.IdRecinto == idRecinto && r.IdProducto == p.IdProducto
                );

                if (recintoProducto == null)
                    continue; // si no hay vínculo, saltar

                // Verificar si se pasó del presupuesto
                bool excedePresupuesto = p.CantidadDisminuida > recintoProducto.ExistenciasActuales;

                if (excedePresupuesto)
                {
                    // Solo para los que se pasan del presupuesto:
                    recintoProducto.ExistenciasActuales = 0;
                    _unitOfWork.RecintoProducto.Update(recintoProducto);

                    // Resta del inventario general del proyecto
                    var inventario = _unitOfWork.Inventario.GetID(
                        i => i.IdProducto == p.IdProducto && i.IdProyecto == idProyecto
                    );

                    if (inventario != null)
                    {
                        inventario.Existencias -= p.CantidadDisminuida;
                        if (inventario.Existencias < 0)
                            inventario.Existencias = 0; // seguridad
                        _unitOfWork.Inventario.Update(inventario);
                    }
                }
            }

            // Cambiar estado de la factura
            var factura = _unitOfWork.Factura.GetID(f => f.IdFactura == idFactura);
            if (factura != null)
            {
                factura.EstadoFactura = 1; // Finalizada
                _unitOfWork.Factura.Update(factura);
            }

            // Cambiar estado del cambio
            cambio.Estado = "Aprobado";
            _unitOfWork.Cambio.Update(cambio);

            // Guardar todo
            _unitOfWork.Save();

            TempData["Success"] = "Solicitud de cambio aprobada. Se ajustaron los productos que excedían el presupuesto.";

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Rechazar(int IdCambio)
        {
            var cambio = _unitOfWork.Cambio.GetID(
                c => c.IdCambio == IdCambio,
                includeProperties: "Factura"
            );

            if (cambio == null)
                return NotFound();

            // 🔹 Cambiar estado del cambio
            cambio.Estado = "Rechazado";
            _unitOfWork.Cambio.Update(cambio);

            // 🔹 Cambiar estado de la factura a 3 (rechazada)
            var factura = _unitOfWork.Factura.GetID(f => f.IdFactura == cambio.IdFactura);
            if (factura != null)
            {
                factura.EstadoFactura = 3;
                _unitOfWork.Factura.Update(factura);
            }

            // 🔹 Guardar cambios
            _unitOfWork.Save();

            TempData["Info"] = "La solicitud de cambio ha sido rechazada. No se realizaron modificaciones al inventario.";

            return RedirectToAction("Index");
        }


    }
}
