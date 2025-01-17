using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Config
{
    [Table("busqueda_parametro", Schema = "config")]
    public class BusquedaParametro
    {
        public enum Tipo
        {
            Numerico = 1,
            Texto = 2,
            Fecha = 3,
            Multivaluado = 4
        }
        [Key]
        public long id { get; set; }
        public long id_busqueda { get; set; }
        [Display(Name = "Nombre Visible"), Required(ErrorMessage = "Campo requerido!")]
        public string nombre_visible { get; set; }
        [Display(Name = "Campo DB"), Required(ErrorMessage = "Campo requerido!")]
        public string campo_bd { get; set; }
        [Display(Name = "Tipo Dato"), Required(ErrorMessage = "Campo requerido!")]
        public Tipo tipo_dato { get; set; }
        [Display(Name = "Query Multivaluado"), Required(ErrorMessage = "Campo requerido!")]
        public string query_multivaluado { get; set; }
        [Display(Name = "Orden"), Required(ErrorMessage = "Campo requerido!")]
        public int orden { get; set; }

        [NotMapped]
        public string _valor_busqueda { get; set; }
    }
}
