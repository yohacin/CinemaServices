using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class UsuarioViewModel : ViewModelBase
    {
        public Entities.Security.Usuario eUsuario { get; set; }
        public List<Entities.Security.Perfil> listaPerfil { get; set; }

        public String stringListaPerfiles { get; set; }
        public List<Entities.Helpers.TreeCollection> listaModuloAcceso { get; set; }

        public UsuarioViewModel() : base()
        {
        }

        public UsuarioViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
