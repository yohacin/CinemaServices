using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("RespuestaWS", Schema = "public")]
    public class RespuestaWS
    {
        [Key]
        public long id { get; set; }
        public string codigo_erp { get; set; }
        public string error { get; set; }
        public string descripcion { get; set; }
        public string jsonCompleto { get; set; }
    }
}
