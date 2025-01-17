using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Login"), Required(AllowEmptyStrings = false, ErrorMessage = "Es requerido el usuario")]
        [StringLength(20, ErrorMessage = "El dato no debe exceder los {1} caracteres")]
        public string login { get; set; }

        [Display(Name = "Password"), Required(AllowEmptyStrings = false, ErrorMessage = "El password es requerido")]
        [StringLength(20, ErrorMessage = "El dato no debe exceder los {1} caracteres")]
        public string password { get; set; }

        public string mensaje { get; set; }

        public LoginViewModel() : base()
        {

        }

    }
}
