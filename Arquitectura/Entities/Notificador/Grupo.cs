using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ENO = Entities.Notificador;

namespace Entities.Notificador
{
    [Table("grupo", Schema = "notificador")]
    public class Grupo
    {
        [Key]
        public long codigo_grupo { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public String nombre { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String descripcion { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public long tipo { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String query { get; set; }
        public bool activo { get; set; }
        public long marca_eliminado { get; set; }
        public bool administrable { get; set; }

        [NotMapped]
        public List<ENO.Contacto> oLstContactos { get; set; }
        [NotMapped]
        public String serializedContactos {
            get
            {
                return "";
            }
            set
            {
                this.oLstContactos = JsonConvert.DeserializeObject<List<Contacto>>(value);
            }
        }
        [NotMapped]
        public int cantidad_contactos {
            get {
                if (this.oLstContactos == null)
                {
                    return 0;
                }

                return this.oLstContactos.Count;
            }
        }

        public Grupo() {
            this.oLstContactos = new List<Contacto>();
            this.query = "";
            this.tipo = 1;
        }
    }
}
