using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Security
{
    [Table("acceso", Schema = "security")]
    public class Acceso
    {
        [Key]
        public long id { get; set; }
        public string nombre { get; set; }
        public string url_pagina { get; set; }
        public bool estado { get; set; }
        public long id_modulo { get; set; }
        public int secuencia { get; set; }
        public string icon { get; set; }
    }
}
