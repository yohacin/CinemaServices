using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("dia_feriado", Schema = "public")]
    public class Dia_Feriado
    {
        [Key]
        public long id { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
        public int estado { get; set; }


        // Cambios modo independiente
        public DateTime? fecha_fin { get; set; }
        public bool se_debe_trasladar { get; set; }
        public bool es_permanente { get; set; }
        public long id_ciudad { get; set; }


        [AutoExpand]
        public Entities.Public.Ciudad ciudad { get; set; }
    }
}
