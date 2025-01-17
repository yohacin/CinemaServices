using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Helpers
{
    public class MarcacionErroneaHelper
    {
        [Key]
        public long id { get; set; }
        public string nombre_empleado{ get; set; }
        public string turno { get; set; }
        public DateTime hora_marcacion{ get; set; }
        public string tipo{ get; set; }

        [NotMapped]
        public string _hora_string { get; set; }
    }
}
