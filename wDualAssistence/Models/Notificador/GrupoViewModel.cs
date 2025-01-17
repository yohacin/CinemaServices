using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models.Notificador
{
    public class GrupoViewModel : ViewModelBase
    {
        public Entities.Notificador.Grupo eGrupo { get; set; }
        public List<Entities.Security.Usuario> listaUsuario { get; set; }
        public string stringListaContacto { get; set; }

        public GrupoViewModel() : base()
        {
        }

        public GrupoViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
