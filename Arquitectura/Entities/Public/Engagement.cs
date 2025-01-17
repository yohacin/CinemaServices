using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("engagement", Schema = "public")]
    public class Engagement
    {
        [Key]
        public long id { get; set; }
        public long id_empresa { get; set; }
        public string titulo { get; set; }
        public string adj_contrato { get; set; }
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public bool facturable { get; set; }
        public int estado_ejecucion { get; set; }
        public int estado { get; set; }
        public long id_area { get; set; }
        public long id_usuario { get; set; }

        [NotMapped]
        public double hora_servicio { get; set; }
        [NotMapped]
        public double hora_extra { get; set; }
        [NotMapped]
        public string text_estado { get; set; }
        [NotMapped]
        public double horas_ejecutadas { get; set; }
        [NotMapped]
        public string empresa { get; set; }
        [NotMapped]
        public string area { get; set; }

        [NotMapped]
        public List<Entities.Public.Detalle_Hora_Engagement> listaDetalle_Hora_Engagement { get; set; }
        
        [NotMapped]
        public List<Entities.Public.Alerta_Engagement> listaAlerta_Engagement { get; set; }

        [NotMapped]
        public List<Entities.Public.Responsable_Engagement> listaResponsable_Engagement { get; set; }

        [NotMapped]
        public List<Entities.Public.Tarea_Engagement> listaTarea_Engagement { get; set; }

    }
}
