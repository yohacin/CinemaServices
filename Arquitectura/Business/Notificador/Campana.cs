using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Entities.Notificador;

#region Librerias Externas

using Microsoft.EntityFrameworkCore;

#endregion Librerias Externas

#region Abreviaturas

using ENO = Entities.Notificador;

using ESE = Entities.Security;

#endregion Abreviaturas

namespace Business.Notificador
{
    public class Campana : Base
    {
        public Campana(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        /// <summary>
        /// Metodo que Guarda un ENO.Campana
        /// </summary>
        /// <param name="eCampana"></param>
        /// <returns>Retorna un bool</returns>
        public bool Guardar(ENO.Campana eCampana)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    eCampana.fecha_ultima_ejecucion = eCampana.fecha_inicial_ejecucion;
                    eCampana.hora_ultima_ejecucion = eCampana.hora_ejecucion;
                    this._DBcontext.Campana.Add(eCampana);
                    this._DBcontext.SaveChanges();

                    foreach (ENO.Grupo grupo in eCampana.oLstGrupo)
                    {
                        ENO.Campana_Detalle detalle = new ENO.Campana_Detalle();
                        detalle.codigo_campana = eCampana.codigo_campana;
                        detalle.codigo_asignado = 1;
                        detalle.marca_eliminado = 0;
                        detalle.codigo_grupo = grupo.codigo_grupo;

                        this._DBcontext.Campana_Detalle.Add(detalle);
                    }

                    if (eCampana.envio_correo)
                    {
                        ENO.Campana_Plantilla campanaPlantilla = new Campana_Plantilla();
                        campanaPlantilla.id_plantilla = eCampana.id_plantilla_correo;
                        campanaPlantilla.codigo_campana = eCampana.codigo_campana;
                        this._DBcontext.Campana_Plantilla.Add(campanaPlantilla);
                    }

                    if (eCampana.envio_mensaje)
                    {
                        ENO.Campana_Plantilla campanaPlantilla = new Campana_Plantilla();
                        campanaPlantilla.id_plantilla = eCampana.id_plantilla_sms;
                        campanaPlantilla.codigo_campana = eCampana.codigo_campana;
                        this._DBcontext.Campana_Plantilla.Add(campanaPlantilla);
                    }

                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Metodo que Guarda un ENO.CampanaDetalle
        /// </summary>
        /// <param name="eCampana"></param>
        /// <returns>Retorna un bool</returns>
        public bool GuardarCampanaDetalle(ENO.Campana_Detalle eCampanaDetalle)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Campana_Detalle.Add(eCampanaDetalle);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Metodo que obtiene una ENO.Campana por su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retorna un objeto ENO.Campana</returns>
        public ENO.Campana Search(long id)
        {
            try
            {
                ENO.Campana eCampana = _DBcontext.Campana.FirstOrDefault(p => p.codigo_campana == id);

                if (eCampana != null)
                {
                    Grupo bGrupo = new Grupo(this._DBcontext);
                    Plantilla bPlantilla = new Plantilla(this._DBcontext);
                    eCampana.oLstGrupo = bGrupo.ListarByCampana(eCampana.codigo_campana);
                    eCampana.oLstPlantillas = bPlantilla.ListarByCampana(eCampana.codigo_campana);

                    if (eCampana.envio_correo)
                    {
                        ENO.Plantilla plantillaCorreo = eCampana.oLstPlantillas.FirstOrDefault(p => p.tipo == ENO.TipoPlantilla.CORREO.StringValue());
                        if (plantillaCorreo != null)
                        {
                            eCampana.id_plantilla_correo = plantillaCorreo.id;
                        }
                    }

                    if (eCampana.envio_mensaje)
                    {
                        ENO.Plantilla plantillaSMS = eCampana.oLstPlantillas.FirstOrDefault(p => p.tipo == ENO.TipoPlantilla.SMS.StringValue());
                        if (plantillaSMS != null)
                        {
                            eCampana.id_plantilla_sms = plantillaSMS.id;
                        }
                    }
                }

                return eCampana;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        private DateTime GetFechaUltimaEjecucion(long codigoCampana)
        {
            try
            {
                String query = $@"Select * From notificador.campana c Where c.codigo_campana = {codigoCampana}";
                //Npgsql.NpgsqlParameter[] parametros = new Npgsql.NpgsqlParameter[] { };
                dynamic result = _DBcontext.CollectionFromSql(query).First();
                //ENO.Campana campana = Helpers.Helpers.ConvertTo<ENO.Campana>(result);
                //DateTime fechaUltimaEjecucion = campana.fecha_ultima_ejecucion.Value;
                DateTime fechaUltimaEjecucion = DateTime.Now;
                return fechaUltimaEjecucion;
            }
            catch (Exception ex)
            {
                //this._log.Error("Error al obtener la fecha de ultima ejecución", ex);
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Metodo que Modifica un ENO.Campana
        /// </summary>
        /// <param name="eCampana"></param>
        /// <returns>Retorna un bool</returns>
        public bool Modificar(ENO.Campana eCampana)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Campana.Update(eCampana);
                    eCampana.hora_ultima_ejecucion = eCampana.hora_ejecucion;
                    //eCampana.fecha_ultima_ejecucion = GetFechaUltimaEjecucion(eCampana.codigo_campana); // evitaba que se ejecute nuevamente la campaña
                    eCampana.fecha_ultima_ejecucion = eCampana.fecha_inicial_ejecucion;
                    this._DBcontext.SaveChanges();

                    this._DBcontext.Campana_Detalle.RemoveRange(this._DBcontext.Campana_Detalle.Where(d => d.codigo_campana == eCampana.codigo_campana));
                    this._DBcontext.SaveChanges();

                    foreach (ENO.Grupo grupo in eCampana.oLstGrupo)
                    {
                        ENO.Campana_Detalle detalle = new ENO.Campana_Detalle();
                        detalle.codigo_campana = eCampana.codigo_campana;
                        detalle.codigo_asignado = 1;
                        detalle.marca_eliminado = 0;
                        detalle.codigo_grupo = grupo.codigo_grupo;

                        this._DBcontext.Campana_Detalle.Add(detalle);
                    }

                    this._DBcontext.Campana_Plantilla.RemoveRange(this._DBcontext.Campana_Plantilla.Where(cp => cp.codigo_campana == eCampana.codigo_campana));

                    if (eCampana.envio_correo)
                    {
                        ENO.Campana_Plantilla campanaPlantilla = new Campana_Plantilla();
                        campanaPlantilla.id_plantilla = eCampana.id_plantilla_correo;
                        campanaPlantilla.codigo_campana = eCampana.codigo_campana;
                        this._DBcontext.Campana_Plantilla.Add(campanaPlantilla);
                    }

                    if (eCampana.envio_mensaje)
                    {
                        ENO.Campana_Plantilla campanaPlantilla = new Campana_Plantilla();
                        campanaPlantilla.id_plantilla = eCampana.id_plantilla_sms;
                        campanaPlantilla.codigo_campana = eCampana.codigo_campana;
                        this._DBcontext.Campana_Plantilla.Add(campanaPlantilla);
                    }

                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Modifica la fecha ejecucion
        /// </summary>
        /// <param name="codigoCampana"></param>
        /// <param name="fechaEjecucion"></param>
        /// <returns>bool</returns>
        public bool ModificarFechaEjecucion(long codigoCampana, DateTime fechaEjecucion)
        {
            try
            {
                ENO.Campana oCampana = Search(codigoCampana);
                if (oCampana != null)
                {
                    DateTime fechaEjecucionFormat = new DateTime(fechaEjecucion.Year, fechaEjecucion.Month, fechaEjecucion.Day, 0, 0, 0, fechaEjecucion.Kind);
                    oCampana.fecha_inicial_ejecucion = fechaEjecucionFormat;
                    oCampana.fecha_final_ejecucion = fechaEjecucionFormat;
                    oCampana.hora_ejecucion = fechaEjecucion.ToString("HH:mm");
                    return Modificar(oCampana);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public long Registrar(DateTime fechaEjecucionCampana, string nombreGrupo, string descripcionGrupoFlujo, string contenido, string remitente, string nombreCampana, bool finalizacion, List<ESE.Usuario> contactos, ENO.Notificacion configuracion, string correoRemitente = "", string contrasenaCorreoRemitente = "", string enlaceInformativo = "", long idEmpresa = 0, long idSpc = 0)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    ENO.Campana campana = new ENO.Campana();
                    campana.nombre = contenido;
                    campana.tipo_envio = 1;

                    DateTime fechaEjecucion = new DateTime(fechaEjecucionCampana.Year, fechaEjecucionCampana.Month, fechaEjecucionCampana.Day, 0, 0, 0, fechaEjecucionCampana.Kind);
                    campana.fecha_inicial_ejecucion = fechaEjecucion;

                    DateTime fechaFinalEjecucion = new DateTime(fechaEjecucionCampana.Year, fechaEjecucionCampana.Month, fechaEjecucionCampana.Day, 0, 0, 0, fechaEjecucionCampana.Kind);
                    campana.fecha_final_ejecucion = fechaFinalEjecucion;

                    campana.fecha_ultima_ejecucion = campana.fecha_inicial_ejecucion;

                    String shoraEjecucion = fechaEjecucionCampana.ToString("HH:mm");
                    campana.hora_ejecucion = shoraEjecucion;
                    campana.cantidad_repeticion = 0;
                    campana.tipo_repeticion = 1;
                    campana.tipo_mensaje = 1;
                    campana.contenido = contenido;
                    campana.nombre_remitente = remitente;
                    campana.correo_remitente = correoRemitente;
                    campana.contrasena_correo_remitente = contrasenaCorreoRemitente;
                    campana.enlace_informativo = enlaceInformativo;

                    campana.envio_correo = configuracion.envia_correo;
                    campana.envio_mensaje = configuracion.envia_sms;

                    campana.envio_whatsapp = false;
                    campana.activo = true;

                    campana.id_empresa = idEmpresa;
                    campana.id_ref = idSpc;

                    campana.marca_eliminado = 0;
                    this._DBcontext.Campana.Add(campana);
                    this._DBcontext.SaveChanges();

                    ENO.Grupo grupo = new ENO.Grupo();
                    grupo.nombre = nombreGrupo;
                    grupo.descripcion = descripcionGrupoFlujo;
                    grupo.tipo = 1;
                    grupo.query = "";
                    grupo.activo = true;
                    grupo.marca_eliminado = 0;
                    this._DBcontext.Grupo.Add(grupo);
                    this._DBcontext.SaveChanges();

                    foreach (ESE.Usuario itemContacto in contactos)
                    {
                        ENO.Contacto contacto = new ENO.Contacto();
                        contacto.codigo_grupo = grupo.codigo_grupo;
                        contacto.codigo_contacto = itemContacto.id;
                        contacto.nombre = itemContacto.nombre;
                        contacto.correo = itemContacto.email != null ? itemContacto.email : "";
                        contacto.telefono = itemContacto.telefono != null ? itemContacto.telefono : "";
                        contacto.tipo = "p";
                        contacto.marca_eliminado = 0;
                        this._DBcontext.Contacto.Add(contacto);
                        this._DBcontext.SaveChanges();
                    }

                    ENO.Campana_Detalle campana_Detalle = new ENO.Campana_Detalle();
                    campana_Detalle.codigo_campana = campana.codigo_campana;
                    campana_Detalle.codigo_asignado = 1;
                    campana_Detalle.marca_eliminado = 0;
                    campana_Detalle.codigo_grupo = grupo.codigo_grupo;
                    this._DBcontext.Campana_Detalle.Add(campana_Detalle);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();

                    return campana.codigo_campana;
                }
                catch (Exception ex)
                {
                    //_log.Error(ex.Message);
                    oTrans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public List<ENO.Campana> ListCampanas()
        {
            DateTime fecha = DateTime.Now.Date;
            return this._DBcontext.Campana.Where(c => c.fecha_inicial_ejecucion <= fecha && c.fecha_final_ejecucion >= fecha && c.marca_eliminado == 0 && c.tipo_envio == 1).ToList<ENO.Campana>();
        }

        public List<ENO.Campana> List()
        {
            List<ENO.Campana> eLstCamapanas = this._DBcontext.Campana.Where(c => c.marca_eliminado == 0).ToList();

            return eLstCamapanas;
        }

        public List<ENO.Campana> ListarByEmpresa(long id_empresa)
        {
            try
            {
                return this._DBcontext.Campana.Where(c => c.marca_eliminado == 0 && c.id_empresa == id_empresa && c.tipo_envio == 3).OrderByDescending(c => c.codigo_campana).ToList();
            }
            catch (Exception ex)
            {
                //_log.Error(ex.Message);
                throw ex;
            }
        }

        public ENO.Campana GetCampanaJefeZonas()
        {
            try
            {
                return this._DBcontext.Campana.FirstOrDefault(o => o.tipo_envio == 2 && o.marca_eliminado == 0);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Obtiene la Campana Jefe Zonas con su Detalle
        /// </summary>
        /// <returns></returns>
        public ENO.Campana GetCampanaJefeZonasDetalle()
        {
            ENO.Campana oCampana = this._DBcontext.Campana.FirstOrDefault(o => o.tipo_envio == 2 && o.marca_eliminado != 9);
            if (oCampana != null)
            {
                DateTime fechaEnvio = (DateTime)oCampana.fecha_inicial_ejecucion;
                string[] sHora = oCampana.hora_ejecucion.Split(":");
                TimeSpan ts = new TimeSpan(int.Parse(sHora[0]), int.Parse(sHora[1]), 0);
                fechaEnvio = fechaEnvio + ts;
                oCampana.fecha_inicial_ejecucion = fechaEnvio;

                oCampana.oLstGrupo = new Grupo(this._DBcontext).ListGrupoByCodCampana(oCampana.codigo_campana);
                foreach (ENO.Grupo oGrupo in oCampana.oLstGrupo)
                {
                    oGrupo.oLstContactos = new Contacto(this._DBcontext).ListContactoByCodGrupo(oGrupo.codigo_grupo);
                }
            }
            return oCampana;
        }

        public bool Eliminar(long id)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    ENO.Campana eCampana = this.Search(id);
                    if (eCampana == null)
                    {
                        throw new Exception("Campaña Inexistente!!...");
                    }
                    eCampana.marca_eliminado = 9;
                    this._DBcontext.Entry(eCampana).State = EntityState.Modified;
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }
    }
}