using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

#region Abreviaturas
using ENO = Entities.Notificador;
using ECO = Entities.Config;
#endregion

namespace wDualAssistence.Models
{
    public class PlantillaViewModel : ViewModelBase
    {
        public List<ENO.Plantilla> eLstPlantilla { get; set; }
        public ENO.Plantilla ePlantilla { get; set; }
        public List<ENO.Clasificador> oLstTipoPlantilla { get; set; }
        public List<ECO.Tipo_Archivo> listaTipoArchivosPermitidos { get; set; }
        public int longitudRutaRecurso { get; set; }
        public string adjuntosUrl { get; set; }

        public PlantillaViewModel() : base()
        {
        }

        public PlantillaViewModel(ClaimsPrincipal user) : base(user)
        {
            this.modulo = "Modulo de Notificador";
            this.programa = "Gestión de Plantillas";

            this.ePlantilla = new ENO.Plantilla();
            this.eLstPlantilla = new List<ENO.Plantilla>();
            this.oLstTipoPlantilla = new List<ENO.Clasificador>();
        }


    }
}
