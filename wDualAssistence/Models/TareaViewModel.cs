using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class TareaViewModel : ViewModelBase
    {
        public Entities.Public.Tarea eTarea { get; set; }
        public List<Entities.Public.Categoria_Tarea> listaCategoria_Tarea { get; set; }

        public TareaViewModel() : base()
        {
        }

        public TareaViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
