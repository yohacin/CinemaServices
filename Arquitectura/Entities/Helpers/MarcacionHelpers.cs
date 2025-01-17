﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Helpers
{
    public class MarcacionHelpers
    {
        [Key]
        public long id { get; set; }
        public string codigo_empleado { get; set; }
        public string nombre_empleado { get; set; }
        public string empresa_entrada { get; set; }
        public string empresa_salida { get; set; }
        public string descripcion_turno { get; set; }
        public string entrada_programada { get; set; }
        public string salida_programada { get; set; }
        public string entrada_real { get; set; }
        public string salida_real { get; set; }
        public double latitud_entrada { get; set; }
        public double longitud_entrada { get; set; }
        public string observacion_punto_entrada { get; set; }
        public double latitud_salida { get; set; }
        public double longitud_salida { get; set; }
        public string observacion_punto_salida{ get; set; }
        public int perimetro_entrada { get; set; }
        public int perimetro_salida { get; set; }
        public string tipo_marcacion_entrada { get; set; }
        public string tipo_marcacion_salida { get; set; }
        [NotMapped]
        public string imagen_ent { get; set; }
        [NotMapped]
        public string imagen_sal { get; set; }
    }
}
