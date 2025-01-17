using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace Entities.Notificador
{
    [Table("adjunto", Schema = "notificador")]
    public class Adjunto
    {
        [Key]
        public long id { get; set; }
        public long id_plantilla { get; set; }
        public string nombre { get; set; }
        public string path { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public long size { get; set; }
        public string alias { get; set; }

        public Adjunto()
        {
        }
    }
}
