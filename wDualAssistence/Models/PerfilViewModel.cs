using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class PerfilViewModel : ViewModelBase
    {
        public Entities.Security.Perfil ePerfil { get; set; }

        public String stringListaPerfiles { get; set; }
        public List<Entities.Helpers.TreeCollection> listaModuloAcceso { get; set; }

        public PerfilViewModel() : base()
        {
        }

        public PerfilViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
