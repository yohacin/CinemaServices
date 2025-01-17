using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class ConfiguracionViewModel : ViewModelBase
    {
        public List<Entities.Public.Categoria_Tarea> listaCategoria_Tarea { get; set; }

        public ConfiguracionViewModel() : base()
        {
        }

        public ConfiguracionViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
