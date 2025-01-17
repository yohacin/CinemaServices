using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class CategoriaTareaViewModel : ViewModelBase
    {
        public Entities.Public.Categoria_Tarea eCategoria_Tarea { get; set; }

        public CategoriaTareaViewModel() : base()
        {
        }

        public CategoriaTareaViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
