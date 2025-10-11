using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class AposentoVM
    {
        public Aposento Aposento { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> NivelList { get; set; }
    }
}
