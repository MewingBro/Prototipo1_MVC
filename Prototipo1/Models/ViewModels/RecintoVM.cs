using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prototipo1.Models.ViewModels
{
    public class RecintoVM
    {
        public Recinto Recinto { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> AposentoList { get; set; }
    }
}
