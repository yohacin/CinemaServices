using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ENO = Entities.Notificador;

namespace Entities.Notificador
{
    [Table("campana", Schema = "notificador")]
    public class Campana
    {
        private String _stringGruposAsignados;
        [Key]
        public long codigo_campana { get; set; }
        [Required(ErrorMessage = "El nombre es requerido!")]
        public String nombre { get; set; }
        public long tipo_envio { get; set; }
        [Required(ErrorMessage = "La fecha inicial de ejecución es requerida!")]
        public DateTime? fecha_inicial_ejecucion { get; set; }
        public DateTime? fecha_final_ejecucion { get; set; }
        [Required(ErrorMessage = "La hora de ejecución es requerida!")]
        public String hora_ejecucion { get; set; }
        public DateTime?  fecha_ultima_ejecucion { get; set; }
        [Required(ErrorMessage = "La cantidad de repetición es requerida!")]
        public long cantidad_repeticion { get; set; }
        public long  tipo_repeticion { get; set; }
        public long tipo_mensaje { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String  contenido { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String nombre_remitente { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String correo_remitente { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String contrasena_correo_remitente { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String enlace_informativo { get; set; }
        public Boolean envio_correo { get; set; }
        public Boolean envio_mensaje { get; set; }
        public Boolean envio_whatsapp { get; set; }
        public Boolean activo { get; set; }
        public long marca_eliminado { get; set; }
        public long id_empresa { get; set; }
        public long id_ref { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String dias_repeticion { get; set; }
        public bool indefinido { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String descripcion { get; set; }
        public String hora_ultima_ejecucion { get; set; }
        [Required(ErrorMessage = "La cantidad de intentos es requerida!")]
        public int cantidad_intentos { get; set; }
        public int contador_intentos { get; set; }
        [Required(ErrorMessage = "El intervalo de intentos es requerido!")]
        public int intervalo_intentos { get; set; }

        [NotMapped]
        public List<ENO.Grupo> oLstGrupo { get; set; }
        [NotMapped]
        public String stringGruposAsignados {
            get
            {
                return this._stringGruposAsignados;
            }
            set {
                this._stringGruposAsignados = value;
                this.oLstGrupo = JsonConvert.DeserializeObject<List<Grupo>>(value);
            }
        }
        [NotMapped]
        public List<ENO.Plantilla> oLstPlantillas { get; set; }
        [NotMapped]
        public List<String> sLstDiasRepeticion {
            get {
                if (this.dias_repeticion == null || this.dias_repeticion == "")
                {
                    return new List<string>();
                }
                return new List<string>(this.dias_repeticion.Split(","));
            }
            set {
                if (value.Count == 0 || value == null)
                {
                    this.dias_repeticion = "";
                } else
                {
                    this.dias_repeticion = String.Join(',', value);
                }
            }
        }
        [NotMapped]
        public long id_plantilla_correo { get; set; }
        [NotMapped]
        public long id_plantilla_sms { get; set; }

        public Campana() {
            this.oLstGrupo = new List<Grupo>();
            this.dias_repeticion = "";
            this.descripcion = "";
            this.hora_ejecucion = "";
            this.hora_ultima_ejecucion = "";
            this.cantidad_intentos = 3;
            this.intervalo_intentos = 60;
        }
    }
}
