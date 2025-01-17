using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class ExcepcionViewModel : ViewModelBase
    {
        public List<Entities.Public.Excepcion> listaExcepcion{ get; set; }

        public ExcepcionViewModel() : base()
        {
        }

        public ExcepcionViewModel(ClaimsPrincipal user) : base(user)
        {
        }

    }
}
