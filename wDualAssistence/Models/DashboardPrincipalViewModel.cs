using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using EHE = Entities.Helpers;
using EPU = Entities.Public;

namespace wDualAssistence.Models
{
    public class DashboardPrincipalViewModel : ViewModelBase
    {
        public DashboardPrincipalViewModel()
        {
        }

        public DashboardPrincipalViewModel(ClaimsPrincipal User) : base(User)
        {
        }

        public EHE.HighchartsReportData dashboardReport { get; set; }
        public List<EPU.Empresa> listaEmpresas { get; set; }
        public List<long> idsEmpresas { get; set; }
    }
}
