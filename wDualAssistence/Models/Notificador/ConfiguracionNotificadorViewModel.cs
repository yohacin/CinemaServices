using Entities.Notificador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using ENO = Entities.Notificador;

namespace wDualAssistence.Models.Notificador
{
    public class ConfiguracionNotificadorViewModel : ViewModelBase
    {
        public ConfiguracionNotificadorViewModel() : base()
        {
        }

        public ConfiguracionNotificadorViewModel(ClaimsPrincipal user) : base(user)
        {
            this.modulo = "Modulo de Notificador";
            this.programa = "Configuración Credenciales SMTP";
        }

        public ENO.Credencial_Correo eCredencial_Correo { get; set; }
    }
}
