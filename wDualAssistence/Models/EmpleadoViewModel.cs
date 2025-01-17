using Entities.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class EmpleadoViewModel : ViewModelBase
    {
        public Entities.Public.Empleado eEmpleado { get; set; }
        public List<Entities.Security.Usuario> listaUsuario { get; set; }
        public List<Entities.Security.Perfil> listaPerfil { get; set; }
        public List<Entities.Public.Jornada> listaJornadas { get; set; }
        public List<Entities.Public.Sucursal> listaSucursales { get; set; }
        public Foto_Empleado foto_empleado { get; set; }

        public EmpleadoViewModel() : base()
        {
        }

        public EmpleadoViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
