using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.OData.ModelBuilder;

namespace Entities.Notificador
{
    [Table("bitacora_sincronizar", Schema = "notificador")]
    public class Bitacora_Sincronizar
    {
        [Key]
        public Int64 id { get; set; }

        public DateTime fecha { get; set; }
        public String metodo { get; set; }
        public Boolean transaccion { get; set; }
        public String observacion { get; set; }
        public String contenido { get; set; }
        public Int64 id_agrupador { get; set; }
        public Int32 intento { get; set; }

        #region Propiedades Auxiliares

        [AutoExpand]
        public DateTime fecha_fin { get; set; }

        [AutoExpand]
        public Int32 cant_metodos { get; set; }

        [AutoExpand]
        public List<Bitacora_Sincronizar> lstBitacora { get; set; }

        #endregion Propiedades Auxiliares

        public Bitacora_Sincronizar()
        {
            //this.lstBitacora = new List<Bitacora_Sincronizar>();
        }
    }
}