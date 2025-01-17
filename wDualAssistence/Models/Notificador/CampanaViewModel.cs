using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

#region Abreviaturas
using ENO = Entities.Notificador;
#endregion

namespace wDualAssistence.Models
{
    public class CampanaViewModel : ViewModelBase
    {
        public List<ENO.Campana> eLstCampana { get; set; }
        public ENO.Campana eCampana { get; set; }
        public List<ENO.Grupo> eLstGruposDisponibles { get; set; }
        public List<ENO.Clasificador> eLstTipoRepeticion { get; set; }
        public List<ENO.Plantilla> eLstPlantillasCorreo { get; set; }
        public List<ENO.Plantilla> eLstPlantillasSMS { get; set; }
        public List<ENO.Clasificador> eLstDiasRepeticion { get; set; }
        public DateTime fechaHoraServidor { get; set; }


        public CampanaViewModel() : base()
        {
            fechaHoraServidor = DateTime.Now;
        }

        public CampanaViewModel(ClaimsPrincipal user) : base(user)
        {
            this.modulo = "Modulo de Notificador";
            this.programa = "Gestión de Campañas";

            this.eLstCampana = new List<ENO.Campana>();
            this.eCampana = new ENO.Campana();
            this.eLstGruposDisponibles = new List<ENO.Grupo>();
            this.eLstTipoRepeticion = new List<ENO.Clasificador>();
            this.eLstPlantillasCorreo = new List<ENO.Plantilla>();
            this.eLstPlantillasSMS = new List<ENO.Plantilla>();
            this.eLstDiasRepeticion = new List<ENO.Clasificador>();
            this.fechaHoraServidor = DateTime.Now;
        }
    }
}
