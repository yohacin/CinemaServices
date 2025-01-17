using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("config_sincronizar", Schema = "notificador")]
    public class Config_Sincronizar
    {
        [Key]
        public int id { get; set; }
        public int id_empresa { get; set; }
        public bool habilitar_sincronizacion_automatica { get; set; }
        public TimeSpan hora_sincronizacion { get; set; }
        public int fecuencia_sincronizar { get; set; }
        public int cantidad_intentos { get; set; }
        public int intervalo_intentos { get; set; }
        public int intervalo_tipo_tiempo { get; set; }
        public bool habilitar_sincronizacion_autorizacion { get; set; }
        public string codigo_autorizacion { get; set; }
        public string ayuda_titulo { get; set; }
        public string ayuda_cuerpo { get; set; }
        public string correo_alerta { get; set; }
        public string correo_copia { get; set; }
    }
}
