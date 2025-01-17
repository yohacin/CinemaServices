using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class MarcacionViewModel : ViewModelBase
    {
        public Entities.Public.Marcacion eMarcacion { get; set; }
        public List<Entities.Public.Empleado> listaEmpleado { get; set; }
        public List<Entities.Public.Empresa> listaEmpresa { get; set; }

        public MarcacionViewModel() : base()
        {
        }

        public MarcacionViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
