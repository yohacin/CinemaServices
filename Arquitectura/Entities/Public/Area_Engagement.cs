using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("area_engagement", Schema = "public")]
    public class Area_Engagement
    {
        [Key]
        public long id { get; set; }
        public string nombre { get; set; }
        public int estado { get; set; }

    }
}
