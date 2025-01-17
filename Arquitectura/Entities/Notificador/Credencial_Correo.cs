using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("credencial_correo", Schema = "notificador")]
    public class Credencial_Correo
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "Seleccione una empresa!")]
        public int id_empresa { get; set; }
        [Required(ErrorMessage = "El host es requerido!")]
        public string host { get; set; }
        [Required(ErrorMessage = "El puerto es requerido!")]
        public int puerto { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es requerido!")]
        public string usuario { get; set; }
        [Required(ErrorMessage = "La contraseña es requerida!")]
        public string contrasena { get; set; }
        public bool enablessl { get; set; }

        public Credencial_Correo()
        {
        }
    }
}
