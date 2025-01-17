using Entities.Public;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Security
{
    [Table("log_sync", Schema = "security")]
    public class LogSync
    {
        [Key]
        public long id { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime fecha { get; set; }
        public string metodo { get; set; }
        public string descripcion { get; set; }
    }
}
