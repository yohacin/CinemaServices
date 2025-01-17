using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Config
{
    [Table("busqueda", Schema = "config")]
    public class Busqueda
    {
        [Key]
        public long id { get; set; }
        [Display(Name = "Nombre Busqueda"), Required(ErrorMessage = "Campo requerido!")]
        public string nombre_busqueda { get; set; }
        [Display(Name = "Descripcion"), Required(ErrorMessage = "Campo requerido!")]
        public string descripcion { get; set; }
        [Display(Name = "query"), Required(ErrorMessage = "Campo requerido!")]
        public string query { get; set; }

        [NotMapped]
        public List<BusquedaParametro> _DetalleParametros { get; set; }

        public Busqueda()
        {
            _DetalleParametros = new List<BusquedaParametro>();
        }

    }
}
