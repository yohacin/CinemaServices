using Microsoft.OData.ModelBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("empleado", Schema = "public")]
    public class Empleado
    {
        [Key]
        public long id { get; set; }

        public string codigo { get; set; }
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public string apellido_casada { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime fecha_nacimiento { get; set; }

        public string telefono { get; set; }
        public string ci { get; set; }
        public string correo { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public string fcm { get; set; }
        public int estado { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.ffff")]
        public DateTime? fecha_ingreso { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.ffff")]
        public DateTime? fecha_salida { get; set; }

        public string area { get; set; }
        public string cargo { get; set; }
        public long id_perfil { get; set; }
        public long id_sucursal { get; set; }
        public string nombre_sucursal { get; set; }

        [AutoExpand]
        public List<Entities.Public.Turno> listaTurno { get; set; }

        [NotMapped]
        public List<List<Entities.Helpers.AsignacionHelper>> listaAsignacionHelper { get; set; }

        [NotMapped]
        public List<Entities.Helpers.HorarioHelper> listaHorario { get; set; }

        [NotMapped]
        public double horas_asignadas { get; set; }

        [NotMapped]
        public List<Entities.Public.Foto_Empleado> listaFotoEmpleado { get; set; }


        
        // Cambios modo independiente
        public long? id_jornada { get; set; }
        //public string codigo_biometrico { get; set; }

        [AutoExpand]
        public Entities.Public.Jornada jornada { get; set; }

        [AutoExpand]
        public Entities.Public.Sucursal sucursal { get; set; }
    }
}