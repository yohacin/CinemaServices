using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("notificacion", Schema = "notificador")]
    public class Notificacion
    {
        [Key]
        public long id { get; set; }
        public String tipo { get; set; }
        public bool envia_correo { get; set; }
        public bool envia_sms { get; set; }
        public long id_nodo { get; set; }
        public bool antes_12 { get; set; }
        public bool antes_24 { get; set; }
        public bool antes_48 { get; set; }
    }
}
