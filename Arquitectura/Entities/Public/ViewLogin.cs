using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    //[Table("ViewLogin", Schema = "public")]
    public class ViewLogin
    {
        [Key]
        public string clave { get; set; }
        public string usuario { get; set; }
        public string action { get; set; }
    }
}
