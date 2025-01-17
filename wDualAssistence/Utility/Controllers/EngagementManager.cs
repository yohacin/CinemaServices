using System.Collections.Generic;
using System;
using BPU = Business.Public;
using Data;
using log4net;

namespace wDualAssistence.Utility.Controllers
{
    public class EngagementManager : MainManager
    {
        public EngagementManager(IApplicationDbContext _appCnx) : base(_appCnx)
        {
        }

        public EngagementManager(IApplicationDbContext _appCnx, ILog _log) : base(_appCnx, _log)
        {
        }

        public bool NotificarAvance()
        {
            try
            {
                //SincronizarByServicio();
                _log.Info("VERIFICANDO Si existena notificacion de avance de engagement");
                List<Entities.Helpers.NotificarAlertaHelper> listaNotificar = new BPU.Engagement(_appCnx).GetNotificarAlertaHelper();

                if (listaNotificar == null) return false;
                foreach (var itemNotificar in listaNotificar)
                {
                    Entities.Notificador.Plantilla ePlantilla = new Business.Notificador.Plantilla(_appCnx).Buscar(Startup.id_plantilla_alerta);
                    if (itemNotificar == null) continue;
                    if (ePlantilla == null) throw new Exception("La plantilla para notificar alerta de avance de engagement no existe!");
                    string template = ePlantilla.contenido;
                    template = template.Replace("{nombre_empresa}", itemNotificar.nombre_empresa);
                    template = template.Replace("{nombre_engagement}", itemNotificar.nombre_engagement);
                    template = template.Replace("{notificar}", itemNotificar.notificar + "");
                    template = template.Replace("{porcentaje_avance}", itemNotificar.porcentaje_avance + "");
                    foreach (var eContacto in itemNotificar.listaContacto)
                    {
                        enviarAlerta(_appCnx, _log, eContacto.correo, template);
                    }
                    new BPU.Engagement(_appCnx).CambiarEstadoEjecutado(itemNotificar.id_alerta);
                }
                return true;
            }
            catch (Exception e)
            {
                enviarAlerta(_appCnx, _log, "billy.crespo@dualbiz.net", "<h1>ERROR NOTIFICAR ALERTAS DE AVANCE EN ENGAGEMENT</h1>" + e.Message + "*****************" + e.ToString());
            }
            return false;
        }

        private void enviarAlerta(IApplicationDbContext _appCnx, log4net.ILog _log, string correo, string contenido)
        {
            try
            {
                Entities.Notificador.Credencial_Correo eCredencial_Correo = new Business.Notificador.Credencial_Correo(_appCnx).GetConfig(1);
                CORREO eCORREO = new CORREO();
                eCORREO._HOST = eCredencial_Correo.host;
                eCORREO._PORT = eCredencial_Correo.puerto;
                eCORREO._USER = eCredencial_Correo.usuario;
                eCORREO._PASS = eCredencial_Correo.contrasena;

                eCORREO.EnviarCorreoContenido(correo, contenido, "NOTIFICADOR");
            }
            catch (Exception e)
            {
                _log.Error("ERROR enviar alerta de engagement", e);
                return;
            }
        }
    }
}