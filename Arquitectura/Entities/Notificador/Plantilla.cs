using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    public enum TipoPlantilla
    {
        CORREO = 1,
        SMS = 2
    }

    [Table("plantilla", Schema = "notificador")]
    public class Plantilla
    {
        [Key]
        public long id { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String contenido { get; set; }
        public String tipo { get; set; }
        public int marca_eliminado { get; set; }
        public long id_empresa { get; set; }
        [Required(ErrorMessage = "Campo requerido!")]
        public String nombre { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public String query { get; set; }
        [NotMapped]
        public bool edicion {
            get
            {
                return this.id > 0 ? true : false;
            }
        }

        [NotMapped]
        [Required(ErrorMessage = "Campo requerido!")]
        public long tipoPlantilla
        {
            get
            {
                if (this.tipo == TipoPlantilla.CORREO.StringValue())
                {
                    return (long)TipoPlantilla.CORREO;
                }
                if (this.tipo == TipoPlantilla.SMS.StringValue())
                {
                    return (long)TipoPlantilla.SMS;
                }
                return 0;
            }

            set
            {
                if (value == (long)TipoPlantilla.CORREO)
                {
                    this.tipo = TipoPlantilla.CORREO.StringValue();
                }
                if (value == (long)TipoPlantilla.SMS)
                {
                    this.tipo = TipoPlantilla.SMS.StringValue();
                }
            }
        }

        [NotMapped]
        public List<Query> oLstQueries { get; set; }
        [NotMapped]
        public string serializedQueries {
            get => "";
            set => this.oLstQueries = JsonConvert.DeserializeObject<List<Query>>(value);
        }
        [NotMapped]
        public List<Adjunto> oLstAdjutos { get; set; }
        [NotMapped]
        public string serializedAdjuntos
        {
            get => "";
            set => this.oLstAdjutos = JsonConvert.DeserializeObject<List<Adjunto>>(value);
        }

        public Plantilla()
        {
            this.query = "";
        }
    }

    public static class EnumExtension
    {
        public static String StringValue(this TipoPlantilla value)
        {
            switch (value)
            {
                case TipoPlantilla.CORREO:
                    return "C";
                case TipoPlantilla.SMS:
                    return "M";
                default:
                    return null;
            }
        }

        public static String StringText(this TipoPlantilla value)
        {
            switch (value)
            {
                case TipoPlantilla.CORREO:
                    return "Correo";
                case TipoPlantilla.SMS:
                    return "SMS";
                default:
                    return null;
            }
        }
    }
}
