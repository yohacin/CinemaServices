using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class DiaFeriadoViewModel : ViewModelBase
    {
        public Entities.Public.Dia_Feriado eDia_Feriado { get; set; }
        public List<Entities.Public.Ciudad> listaCiudades { get; set; }

        public DiaFeriadoViewModel() : base()
        {
        }

        public DiaFeriadoViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
