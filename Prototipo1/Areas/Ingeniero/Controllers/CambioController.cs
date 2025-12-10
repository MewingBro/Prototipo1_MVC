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

            //obtiene datos del usuario logueado
            var userId = _userManager.GetUserId(User);
            var roles = _userManager.GetRolesAsync(_userManager.GetUserAsync(User).Result).Result;
            bool esIngeniero = roles.Contains("Ingeniero");

            IEnumerable<Cambio> cambiosQuery;

            // la lista de cambios varia segun el tipo de usuario logueado
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
                cambiosQuery = cambiosQuery.Where(c => c.Factura.Fecha?.Date == fecha.Value.Date);

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
        public IActionResult Create(int IdFactura,string Comentario)
        {
            // Obtener el usuario actual (Identity)
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Debe iniciar sesión para crear una solicitud de cambio.";
                return RedirectToAction("Login", "Account");
            }

            // Crear la solicitud
            Cambio nuevaSolicitud = new Cambio
            {
                IdFactura = IdFactura,
                Estado = "Pendiente",
                IdUsuario = userId,
                FechaSolicitud = DateTime.Now, 
                Comentario = ""
            };

            // Guardar en base de datos
            _unitOfWork.Cambio.Add(nuevaSolicitud);
            _unitOfWork.Save();

            // Cambiar estado de la factura
            var factura = _unitOfWork.Factura.GetID(f => f.IdFactura == IdFactura);
            if (factura != null)
            {
                factura.EstadoFactura = 2; // Pendiente de aprobacion
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

            // Construir una lista de objetos para mostrar los datos combinados
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
                    Presupuesto = recintoProd.Presupuesto,
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
        public IActionResult Aprobar(int IdCambio, string Comentario)
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

            var productos = _unitOfWork.FacturaSalidaProducto.GetAllBYID(
                f => f.IdFactura == idFactura,
                includeProperties: "Producto"
            ).ToList();

            // Lista para registrar los detalles del cambio
            List<CambioDetalle> detallesExcedidos = new();

            foreach (var p in productos)
            {
                var recintoProducto = _unitOfWork.RecintoProducto.GetID(
                    r => r.IdRecinto == idRecinto && r.IdProducto == p.IdProducto
                );

                if (recintoProducto == null)
                    continue;

                bool excedePresupuesto = p.CantidadDisminuida > recintoProducto.ExistenciasActuales;

                if (excedePresupuesto)
                {
                    // Guardar detalle del producto excedido
                    var detalle = new CambioDetalle
                    {
                        IdCambio = IdCambio,
                        IdProducto = p.IdProducto,
                        Presupuesto = recintoProducto.Presupuesto, // si existe el campo, ajústalo según tu modelo
                        ExistenciasActuales = recintoProducto.ExistenciasActuales,
                        CantidadDisminuida = p.CantidadDisminuida
                    };
                    detallesExcedidos.Add(detalle);

                    // Ajustar existencias a cero en el recinto
                    recintoProducto.ExistenciasActuales = 0;
                    _unitOfWork.RecintoProducto.Update(recintoProducto);

                    // Actualizar inventario general del proyecto
                    var inventario = _unitOfWork.Inventario.GetID(
                        i => i.IdProducto == p.IdProducto && i.IdProyecto == idProyecto
                    );

                    if (inventario != null)
                    {
                        inventario.Existencias -= p.CantidadDisminuida;
                        if (inventario.Existencias < 0)
                            inventario.Existencias = 0;

                        _unitOfWork.Inventario.Update(inventario);
                    }
                }
            }

            // Guardar todos los detalles de productos excedidos
            if (detallesExcedidos.Any())
            {
                foreach (var d in detallesExcedidos)
                {
                    _unitOfWork.CambioDetalle.Add(d);
                }
            }

            // Cambiar estado de la factura y guardar comentario
            var factura = _unitOfWork.Factura.GetID(f => f.IdFactura == idFactura);
            if (factura != null)
            {
                factura.EstadoFactura = 1; // Finalizada
                _unitOfWork.Factura.Update(factura);
            }

            cambio.Estado = "Aprobado";
            cambio.Comentario = Comentario;
            _unitOfWork.Cambio.Update(cambio);

            _unitOfWork.Save();

            TempData["Success"] = "Solicitud de cambio aprobada. Se ajustaron los productos que excedían el presupuesto.";
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult Rechazar(int IdCambio, string Comentario)
        {
            var cambio = _unitOfWork.Cambio.GetID(
                c => c.IdCambio == IdCambio,
                includeProperties: "Factura"
            );

            if (cambio == null)
                return NotFound();

            cambio.Estado = "Rechazado";
            cambio.Comentario = Comentario; 
            _unitOfWork.Cambio.Update(cambio);

            var factura = _unitOfWork.Factura.GetID(f => f.IdFactura == cambio.IdFactura);
            if (factura != null)
            {
                factura.EstadoFactura = 3; // no se efectuan cambios y la factura se muestra en estado rechazada
                _unitOfWork.Factura.Update(factura);
            }

            _unitOfWork.Save();

            TempData["Info"] = "La solicitud de cambio ha sido rechazada. No se realizaron modificaciones al inventario.";
            return RedirectToAction("Index");
        }

        public IActionResult ReporteCambios(string nivel, string aposento, string recinto, string codigo, string nombre)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");

            if (idProyecto == null)
            {
                TempData["Error"] = "No se ha seleccionado un proyecto activo.";
                return RedirectToAction("Index", "Home");
            }

            // Cargar los cambios aprobados del proyecto activo
            var cambios = _unitOfWork.Cambio.GetAllBYID(
                c => c.Estado == "Aprobado" && c.Factura.IdProyecto == idProyecto,
                includeProperties: "Factura,Factura.Recinto,Factura.Proyecto,Factura.Recinto.Aposento.Nivel"
            ).ToList();

            var detalles = _unitOfWork.CambioDetalle.GetAll(
                includeProperties: "Producto"
            ).ToList();

            // Construir el resultado uniendo los datos necesarios
            var resultado = (from c in cambios
                             join d in detalles on c.IdCambio equals d.IdCambio
                             let rec = c.Factura.Recinto
                             select new CambioPresupuestoVM
                             {
                                 IdFactura = c.IdFactura,
                                 CodigoProducto = d.Producto.CodigoProducto,
                                 NombreProducto = d.Producto.NombreProducto,
                                 Presupuesto = d.Presupuesto,
                                 CantidadDisminuida = d.CantidadDisminuida,
                                 Nivel = rec?.Aposento?.Nivel?.NombreNivel ?? "-",
                                 Aposento = rec?.Aposento?.NombreAposento ?? "-",
                                 Recinto = rec?.NombreRecinto ?? "-",
                                 Comentario = c.Comentario
                             }).ToList();

            // Aplicar filtros dinámicos
            if (!string.IsNullOrWhiteSpace(nivel))
                resultado = resultado.Where(x => x.Nivel.Contains(nivel, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(aposento))
                resultado = resultado.Where(x => x.Aposento.Contains(aposento, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(recinto))
                resultado = resultado.Where(x => x.Recinto.Contains(recinto, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(codigo))
                resultado = resultado.Where(x => x.CodigoProducto.Contains(codigo, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(nombre))
                resultado = resultado.Where(x => x.NombreProducto.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.IdProyecto = idProyecto;
            return View(resultado);
        }

        public IActionResult ImprimirReporteCambios(string nivel, string aposento, string recinto, string codigo, string nombre)
        {
            int? idProyecto = HttpContext.Session.GetInt32("IdProyecto");
            if (idProyecto == null)
            {
                TempData["Error"] = "No se ha seleccionado un proyecto activo.";
                return RedirectToAction("Index", "Home");
            }

            var cambios = _unitOfWork.Cambio.GetAllBYID(
                c => c.Estado == "Aprobado" && c.Factura.IdProyecto == idProyecto,
                includeProperties: "Factura,Factura.Recinto,Factura.Recinto.Aposento.Nivel"
            ).ToList();

            var detalles = _unitOfWork.CambioDetalle.GetAll(includeProperties: "Producto").ToList();

            var resultado = (from c in cambios
                             join d in detalles on c.IdCambio equals d.IdCambio
                             let rec = c.Factura.Recinto
                             select new CambioPresupuestoVM
                             {
                                 IdFactura = c.IdFactura,
                                 CodigoProducto = d.Producto.CodigoProducto,
                                 NombreProducto = d.Producto.NombreProducto,
                                 Presupuesto = d.Presupuesto,
                                 CantidadDisminuida = d.CantidadDisminuida,
                                 Nivel = rec?.Aposento?.Nivel?.NombreNivel ?? "-",
                                 Aposento = rec?.Aposento?.NombreAposento ?? "-",
                                 Recinto = rec?.NombreRecinto ?? "-",
                                 Comentario = c.Comentario
                             }).ToList();

            // Aplicar filtros igual que en el action principal
            if (!string.IsNullOrWhiteSpace(nivel))
                resultado = resultado.Where(x => x.Nivel.Contains(nivel, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(aposento))
                resultado = resultado.Where(x => x.Aposento.Contains(aposento, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(recinto))
                resultado = resultado.Where(x => x.Recinto.Contains(recinto, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(codigo))
                resultado = resultado.Where(x => x.CodigoProducto.Contains(codigo, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(nombre))
                resultado = resultado.Where(x => x.NombreProducto.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();

            var proyecto = _unitOfWork.Proyecto.GetID(p => p.IdProyecto == idProyecto);
            ViewBag.Proyecto = proyecto?.NombreProyecto ?? "Proyecto desconocido";

            return View("ReporteCambiosImprimir", resultado);
        }


    }
}
