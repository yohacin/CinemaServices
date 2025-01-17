using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Helpers
{
    public class NotificarAlertaHelper
    {
        [Key]
        public long id_alerta { get; set; }
        public string nombre_empresa { get; set; }
        public long id_engagement { get; set; }
        public string nombre_engagement { get; set; }
        public double notificar { get; set; }
        public double porcentaje_avance { get; set; }

        [NotMapped]
        public List<Entities.Notificador.Contacto> listaContacto { get; set; }

    }
}
