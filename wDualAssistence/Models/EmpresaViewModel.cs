using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class EmpresaViewModel : ViewModelBase
    {
        public Entities.Public.Empresa eEmpresa { get; set; }
        public String stringListaPerfiles { get; set; }
        public List<Entities.Public.Empleado> listaEmpleado { get; set; }
        public List<Entities.Public.Engagement> listaEngagement { get; set; }
        public long id_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public EmpresaViewModel() : base()
        {
        }

        public EmpresaViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
