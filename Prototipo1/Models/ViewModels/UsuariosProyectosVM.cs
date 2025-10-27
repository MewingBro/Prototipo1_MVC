using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class UsuariosProyectosVM
    {
        public UsuariosProyectos UsuariosProyectos { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> UsuarioList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ProyectoList { get; set; }

    }
}
