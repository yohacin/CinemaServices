using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using wDualAssistence.Helpers;

namespace wDualAssistence.Models
{
    public class ViewModelBase
    {
        public string NombreUsuario { get; }
        public string UsuarioImagen { get; }
        public string Contrasena { get; }
        public string Correo { get; }
        public string NombreEmpresa { get; }
        public string EmpresaImagen { get; }
        public string NombreRol { get; }
        public long idUsuario { get; }
        public long idEmpresa { get; }
        public string dashboardEmpresa { get; }
        public List<object> menuItems { get; set; }
        public string programaSeleccionado { get; set; }
        public string ip { get; }
        public int tipo { get; }
        public int dashboard { get; }

        //Bitacora
        public string modulo { get; set; }
        public string programa { get; set; }
        public string sJsonObj { get; set; }

        public String errorMsg { get; set; }

        public enum Accion
        {
            //Bitacora
            Guardar,
            Modificar,
            Eliminar,
            CambioContraseña,
            Dar_Alta,
            Dar_Baja,
            Reasignar
        }

        public ViewModelBase(ClaimsPrincipal User)
        {
            //Opteniendo datos de la cookie

            NombreUsuario = User.GetClaimValue("nombre_usuario");
            Contrasena = User.GetClaimValue("contrasena");
            Correo = User.GetClaimValue("correo");
            UsuarioImagen = User.GetClaimValue("imagen_usuario");

            long idRol = 123;
            idUsuario = Convert.ToInt64(User.GetClaimValue("idUsuario"));
            tipo = Convert.ToInt32(User.GetClaimValue("tipo"));
            dashboard = Convert.ToInt32(User.GetClaimValue("dashboard"));
            menuItems = new List<object>();

            //programaSeleccionado = User.GetClaimValue("programa_seleccionado");

            ip = "127.0.0.0.1";//User.GetClaimValue("ip");

        }


        public ViewModelBase()
        {

        }
    }
}
