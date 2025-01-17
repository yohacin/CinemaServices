using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Security
{
    [Table("usuario", Schema = "security")]
    public class Usuario
    {
        [Key]
        public long id { get; set; }

        [Display(Name = "Nombre Completo"), Required(ErrorMessage = "Campo requerido!")]
        public string nombre { get; set; }

        [Display(Name = "Nombre Usuario"), Required(ErrorMessage = "Campo requerido!")]
        public string usuario { get; set; }

        [Display(Name = "Contraseña"), Required(ErrorMessage = "Campo requerido!")]
        public string clave { get; set; }

        public string ci_nit { get; set; }

        [Display(Name = "Correo"), Required(ErrorMessage = "Campo requerido!")]
        public string email { get; set; }

        [Display(Name = "Teléfono"), Required(ErrorMessage = "Campo requerido!")]
        [StringLength(10, ErrorMessage = "El dato no debe exceder los {1} caracteres")]
        public string telefono { get; set; }

        public long id_perfil { get; set; }
        public bool estado { get; set; }
        public bool dashboard { get; set; }
        public string foto { get; set; }
        public DateTime updated_at { get; set; }
        public long updated_by { get; set; }
        public bool baja { get; set; }
        public bool configuracion { get; set; }

        [AutoExpand]
        public string perfil { get; set; }

        [NotMapped]
        public List<Entities.Security.Usuario_Acceso> listaUsuario_Acceso { get; set; }
    }
}