using Data;
using Entities.Public;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using BSE = Business.Security;
using EHE = Entities.Helpers;
using EPU = Entities.Public;
using ESE = Entities.Security;

namespace Business.Public
{
    public class Empleado : Base, IBusiness<EPU.Empleado>
    {
        public Empleado(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Empleado eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    EPU.Empleado eEmpleadoAux = new Empleado(this._DBcontext).BuscarXCI(eEntidad.ci);
                    if (eEmpleadoAux == null)
                    {
                        eEntidad.id = 0;
                        this._DBcontext.Empleado.Add(eEntidad);
                        this._DBcontext.SaveChanges();
                    }
                    else
                    {
                        eEntidad.id = eEmpleadoAux.id;
                        eEntidad.id_perfil = eEmpleadoAux.id_perfil;
                        this._DBcontext.Empleado.Update(eEntidad);
                        this._DBcontext.SaveChanges();
                    }

                    //si mandaron turnos para modificar
                    if (eEntidad.listaHorario != null && eEntidad.listaHorario.Count > 0)
                    {
                        List<EPU.Turno> lstTurnos = _DBcontext.Turno.Where(t => t.id_empleado == eEntidad.id).ToList();
                        _DBcontext.Turno.RemoveRange(lstTurnos);
                        _DBcontext.SaveChanges();

                        /*foreach (EPU.Turno eTurno in eEntidad.listaTurno)
                        {
                            eTurno.descripcion_turno = eTurno.descripcion_turno + "(" + eTurno.entrada.ToString("HH:mm") + " - " + eTurno.salida.ToString("HH:mm") + ")";
                            eTurno.id_empleado = eEntidad.id; //relacion con codigo de empleado

                            bTurno.Guardar(eTurno);
                        }*/

                        BSE.Clasificador bClasificador = new BSE.Clasificador(this._DBcontext);
                        const long tipoDiaSemana = 4;
                        List<ESE.Clasificador> dias = bClasificador.ListarPorTipo(tipoDiaSemana);

                        foreach (EHE.HorarioHelper horario in eEntidad.listaHorario)
                        {
                            EPU.Turno turno = new EPU.Turno()
                            {
                                id_empleado = eEntidad.id,
                                codigo_turno = horario.codigo_horario,
                                entrada = horario.hora_entrada,
                                salida = horario.hora_salida,
                                descripcion_turno = $"Turno {horario.codigo_horario} - {dias.Find(d => Convert.ToInt32(d.valor) == horario.codigo_dia).descripcion} ({horario.hora_entrada.ToString("HH:mm")} - {horario.hora_salida.ToString("HH:mm")})",
                                anticipo_entrada = (int)((horario.hora_entrada - horario.hora_inicio_entrada).TotalHours * 60),
                                tolerancia_entrada = (int)((horario.hora_fin_entrada - horario.hora_entrada).TotalHours * 60),
                                anticipo_salida = (int)((horario.hora_salida - horario.hora_inicio_salida).TotalHours * 60),
                                tolerancia_salida = (int)((horario.hora_fin_salida - horario.hora_salida).TotalHours * 60),
                                idc_dia = horario.codigo_dia
                            };

                            this._DBcontext.Turno.Add(turno);
                            this._DBcontext.SaveChanges();
                        }

                        //habilitando marcaciones
                        bool registrarMarcacion = true;
                        EPU.Empleado eEmpleado = eEntidad;

                        DateTime FechaServidor = DateTime.Now;
                        DateTime FechaNULL = new DateTime(1900, 1, 1, 0, 0, 0, FechaServidor.Kind);

                        if (eEmpleado.fecha_ingreso != FechaNULL)
                        {
                            //fecha ingreso mayor a la fecha actual, TODAVIA NO MARCA
                            if (eEmpleado.fecha_ingreso > FechaServidor)
                                registrarMarcacion = false;
                        }

                        if (eEmpleado.fecha_salida != FechaNULL)
                        {
                            //fecha del servidor es mayor a la fecha de retiro del empleado (YA NO DEBE MARCAR)
                            DateTime fechaSalidaEmpleado = eEmpleado.fecha_salida.Value.AddDays(1);
                            if (FechaServidor > fechaSalidaEmpleado)
                                registrarMarcacion = false;
                        }

                        if (registrarMarcacion)
                        {
                            Marcacion bMarcacion = new Marcacion(this._DBcontext);
                            DateTime fechaHOY = DateTime.Now;
                            foreach (EPU.Turno eTurno in eEmpleado.listaTurno)
                            {
                                EPU.Marcacion eMarcacionXDefecto = new EPU.Marcacion();

                                DateTime fechaEntradaProgramada = new DateTime(fechaHOY.Year, fechaHOY.Month, fechaHOY.Day,
                                                                                eTurno.entrada.Hour, eTurno.entrada.Minute, eTurno.entrada.Second, fechaHOY.Kind);

                                DateTime fechaSalidaProgramada = new DateTime(fechaHOY.Year, fechaHOY.Month, fechaHOY.Day,
                                                                               eTurno.salida.Hour, eTurno.salida.Minute, eTurno.salida.Second, fechaHOY.Kind);

                                eMarcacionXDefecto.id_empleado = eEmpleado.id;
                                eMarcacionXDefecto.codigo_turno = eTurno.codigo_turno;
                                eMarcacionXDefecto.entrada_programada = fechaEntradaProgramada; //entrada programada
                                eMarcacionXDefecto.salida_programada = fechaSalidaProgramada; //salida programada
                                eMarcacionXDefecto.latitud_entrada = 0D;
                                eMarcacionXDefecto.longitud_entrada = 0D;
                                eMarcacionXDefecto.latitud_salida = 0D;
                                eMarcacionXDefecto.longitud_salida = 0D;

                                EPU.Marcacion eAux = new Marcacion(this._DBcontext).BuscarMarcacion(eMarcacionXDefecto);
                                if (eAux == null)
                                {
                                    this._DBcontext.Marcacion.Add(eMarcacionXDefecto);
                                    this._DBcontext.SaveChanges();
                                }
                            }
                        }
                    }

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

        public bool Modificar(EPU.Empleado eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Empleado.Update(eEntidad);
                    this._DBcontext.SaveChanges();

                    //eliminar accesos anteriores
                    //this._DBcontext.Turno.RemoveRange(this._DBcontext.Turno.Where(e => e.id_empleado == eEntidad.id));
                    //this._DBcontext.SaveChanges();

                    //guardar los accesos
                    //if (eEntidad.listaTurno != null)
                    //{
                    //    eEntidad.listaTurno = eEntidad.listaTurno.Select(e => { e.id_empleado = eEntidad.id; return e; }).ToList();
                    //    this._DBcontext.Turno.AddRange(eEntidad.listaTurno);
                    //    this._DBcontext.SaveChanges();
                    //}

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



        public bool GuardarFotos(long id_empleado, List<Entities.Public.Foto_Empleado> listaFoto)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    var listaEliminar = this._DBcontext.Foto_Empleado.Where(e => e.id_empleado == id_empleado).ToList();
                    this._DBcontext.Foto_Empleado.RemoveRange(listaEliminar);
                    this._DBcontext.SaveChanges();

                    this._DBcontext.Foto_Empleado.AddRange(listaFoto);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    throw;
                }
            }

        }
        public bool setContrasena(long id, String nueva_contrasena)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    EPU.Empleado eEmpleado = this.GetEntity(id);
                    eEmpleado.clave = nueva_contrasena;
                    this._DBcontext.Entry(eEmpleado).State = EntityState.Modified;
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

        public bool Eliminar(long id)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    EPU.Empleado eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Empleado Inexistente!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Empleado.Update(eEntidad);
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

        public EPU.Empleado GetEntity(long id)
        {
            try
            {
                return _DBcontext.Empleado.FirstOrDefault(p => p.estado == 0 && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Empleado GetEntityConTurno(long id)
        {
            try
            {
                var eEmpleado = _DBcontext.Empleado.FirstOrDefault(p => p.estado == 0 && p.id == id);
                if (eEmpleado != null)
                {
                    eEmpleado.listaTurno = this._DBcontext.Turno.Where(t => t.id_empleado == eEmpleado.id).ToList();

                    if (eEmpleado.listaTurno.Count > 0)
                    {
                        const long tipoDiaSemana = 4;

                        List<int> idcDias = eEmpleado.listaTurno
                            .Where(t => t.idc_dia != null)
                            .Select(t => t.idc_dia.Value).ToList();

                        List<ESE.Clasificador> diasSemana = _DBcontext.ClasificadorSecuriry
                        .Where(cs => cs.id_tipo_clasificador == tipoDiaSemana && idcDias.Contains(Convert.ToInt32(cs.valor))).ToList();

                        eEmpleado.listaTurno
                            .Where(t => t.idc_dia != null).ToList()
                            .ForEach(t => t.dia = diasSemana.Find(d => Convert.ToInt32(d.valor) == t.idc_dia));

                        eEmpleado.listaTurno = eEmpleado.listaTurno.OrderBy(t => t.idc_dia).ToList();
                    }
                }

                return eEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empleado> GetLista()
        {
            try
            {
                var listaEmpleado = _DBcontext.Empleado.Where(u => u.estado == 0).ToList();
                if (listaEmpleado != null)
                {
                    listaEmpleado = listaEmpleado.Select(e =>
                    {
                        e.nombre = e.nombre + " " + e.apellido_paterno + " " + e.apellido_materno;
                        e.listaTurno = this._DBcontext.Turno.Where(t => t.id_empleado == e.id).ToList();
                        return e;
                    }).ToList();
                }

                return listaEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw;
            }
        }

        public List<EPU.Empleado> GetListaSinTurno()
        {
            try
            {
                var listaEmpleado = _DBcontext.Empleado.Where(u => u.estado == 0).ToList();
                if (listaEmpleado != null)
                {
                    listaEmpleado = listaEmpleado.Select(e =>
                    {
                        e.nombre = e.nombre + " " + e.apellido_paterno + " " + e.apellido_materno;
                        return e;
                    }).ToList();
                }
                return listaEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empleado> GetListaByArea(string area)
        {
            try
            {
                var listaEmpleado = _DBcontext.Empleado.Where(u => u.estado == 0 && u.area.ToUpper() == area.ToUpper()).ToList();
                if (listaEmpleado != null)
                {
                    listaEmpleado = listaEmpleado.Select(e =>
                    {
                        e.nombre = e.nombre + " " + e.apellido_paterno + " " + e.apellido_materno;
                        return e;
                    }).ToList();
                }

                return listaEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empleado> GetListaAsignacion(long id_empresa)
        {
            try
            {
                string sQueryEmpresa = $@"
                    SELECT *
                    FROM PUBLIC.empleado
                    WHERE estado = 0 and id in (
                        SELECT id_empleado FROM PUBLIC.empleado_empresa
                    )
                ";
                var listaEmpleado = _DBcontext.Empleado.FromSqlRaw(sQueryEmpresa).ToList();
                if (listaEmpleado != null)
                {
                    foreach (var itemEmpleado in listaEmpleado)
                    {
                        string sQuery = $@"
SELECT
	eng_emp.id id_engagement_empleado, empr.id id_empresa, empr.nombre nombre_empresa,
	eng.id id_engagement, eng.titulo nombre_engagement,
COALESCE(
	SUM((
		SELECT SUM(cantidad_horas)
		FROM PUBLIC.detalle_engagement_empleado
		WHERE id_engagement_empleado = eng_emp.id and estado = 0
	))
, 0) as horas_asignadas,
COALESCE(
	SUM((
		SELECT SUM(hora)
		FROM PUBLIC.empleado_hoja_trabajo
		WHERE id_engagement = eng.id and estado = 0
	))
,0 ) as horas_utilizadas,
	0 horas_disponibles
FROM PUBLIC.empresa empr
JOIN PUBLIC.engagement eng on eng.id_empresa = empr.id
JOIN PUBLIC.engagement_empleado eng_emp on eng_emp.id_engagement = eng.id
WHERE eng.estado = 0 and eng_emp.estado = 0 and eng_emp.id_empleado = {itemEmpleado.id}
GROUP BY eng_emp.id, empr.id, empr.nombre, eng.id, eng.titulo
                        ";

                        var listsAsignacionDetalle = this._DBcontext.AsignacionHelper.FromSqlRaw(sQuery).ToList();
                        if (listsAsignacionDetalle != null)
                        {
                            listsAsignacionDetalle = listsAsignacionDetalle.Select(dtl =>
                            {
                                dtl.horas_disponibles = dtl.horas_asignadas - dtl.horas_utilizadas;
                                return dtl;
                            }).ToList();
                        }
                        if (itemEmpleado.area == null) itemEmpleado.area = "SIN AREA";

                        itemEmpleado.listaAsignacionHelper = listsAsignacionDetalle
                          .GroupBy(u => u.id_empresa)
                          .Select(grp => grp.ToList())
                          .ToList();
                    }
                }

                return listaEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Turno> GetTurnoByEmpleado(long id_empleado)
        {
            try
            {
                return _DBcontext.Turno.Where(e => e.id_empleado == id_empleado).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Empleado Login(string usuario, string contrasena)
        {
            try
            {
                EPU.Empleado eEmpleado = _DBcontext.Empleado.FirstOrDefault(u => u.usuario.ToUpper() == usuario.ToUpper() && u.clave == contrasena);
                return eEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Empleado BuscarXUsuario(string usuario)
        {
            try
            {
                EPU.Empleado eEmpleado = _DBcontext.Empleado.FirstOrDefault(u => u.usuario.ToUpper() == usuario.ToUpper());
                return eEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Empleado BuscarXCodigoEmpleado(string pCodigo)
        {
            try
            {
                EPU.Empleado eEmpleado = _DBcontext.Empleado.FirstOrDefault(u => u.codigo == pCodigo);
                return eEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Empleado BuscarXCI(string pCI, bool swCargarFoto = false)
        {
            try
            {
                var eEmpleado = _DBcontext.Empleado.FirstOrDefault(u => u.ci.Trim() == pCI);
                //if (eEmpleado == null) throw new Exception($"No existe empleado con C.I.: {pCI}.");
                //if (swCargarFoto) {
                //    eEmpleado.listaFotoEmpleado = this._DBcontext.Foto_Empleado.Where(e => e.id_empleado == eEmpleado.id).ToList();
                //}

                return eEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }



        public List<EPU.Empleado> listarEmpleadosSinMarcacionRegistrada(DateTime pFecha)
        {
            try
            {
                string query = $@"SELECT *
                                 FROM public.empleado
                                 WHERE  id not in(
                                        SELECT DISTINCT  id_empleado
                                        FROM public.marcacion
                                        WHERE entrada_programada :: DATE = DATE('{pFecha}'))
                                    AND estado = 0
                                 ORDER BY(
                                    nombre || ' ' || apellido_paterno || ' ' || apellido_materno);";

                List<EPU.Empleado> lstEmpleados = _DBcontext.Empleado.FromSqlRaw(query).ToList();
                Turno bTurno = new Turno(this._DBcontext);
                foreach (EPU.Empleado eEmpleado in lstEmpleados)
                {
                    List<EPU.Turno> lstTurnos = bTurno.listarTurnosEmpleado(eEmpleado.id);
                    eEmpleado.listaTurno = lstTurnos;
                }

                return lstEmpleados;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool VerificarEstadoSalida()
        {
            try
            {
                string sQuery = $"UPDATE empleado set estado = 9 WHERE '{DateTime.Now.ToString("yyyy-MM-dd")}' > fecha_salida::DATE";
                this._DBcontext.Database.ExecuteSqlRaw(sQuery);
                this._DBcontext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }


        #region Metodos Modo Independiente

        public bool GuardarNuevoEmpleado(EPU.Empleado eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    ValidarDatosEmpleado(eEntidad, true);

                    eEntidad.id = 0;
                    this._DBcontext.Empleado.Add(eEntidad);
                    this._DBcontext.SaveChanges();

                    //si mandaron turnos para modificar
                    if (eEntidad.listaHorario != null && eEntidad.listaHorario.Count > 0)
                    {
                        List<EPU.Turno> lstTurnos = _DBcontext.Turno.Where(t => t.id_empleado == eEntidad.id).ToList();
                        _DBcontext.Turno.RemoveRange(lstTurnos);
                        _DBcontext.SaveChanges();

                        BSE.Clasificador bClasificador = new BSE.Clasificador(this._DBcontext);
                        const long tipoDiaSemana = 4;
                        List<ESE.Clasificador> dias = bClasificador.ListarPorTipo(tipoDiaSemana);

                        // Nueva lista turnos, para los nuevos turnos a crearse
                        eEntidad.listaTurno = new List<EPU.Turno>();

                        foreach (EHE.HorarioHelper horario in eEntidad.listaHorario)
                        {
                            EPU.Turno turno = new EPU.Turno()
                            {
                                id_empleado = eEntidad.id,
                                codigo_turno = horario.codigo_horario,
                                entrada = horario.hora_entrada,
                                salida = horario.hora_salida,
                                descripcion_turno = $"Turno {horario.codigo_horario} - {dias.Find(d => Convert.ToInt32(d.valor) == horario.codigo_dia).descripcion} ({horario.hora_entrada.ToString("HH:mm")} - {horario.hora_salida.ToString("HH:mm")})",
                                anticipo_entrada = (int)((horario.hora_entrada - horario.hora_inicio_entrada).TotalHours * 60),
                                tolerancia_entrada = (int)((horario.hora_fin_entrada - horario.hora_entrada).TotalHours * 60),
                                anticipo_salida = (int)((horario.hora_salida - horario.hora_inicio_salida).TotalHours * 60),
                                tolerancia_salida = (int)((horario.hora_fin_salida - horario.hora_salida).TotalHours * 60),
                                idc_dia = horario.codigo_dia
                            };

                            this._DBcontext.Turno.Add(turno);
                            this._DBcontext.SaveChanges();

                            eEntidad.listaTurno.Add(turno);
                        }

                        //habilitando marcaciones
                        bool registrarMarcacion = true;
                        EPU.Empleado eEmpleado = eEntidad;

                        DateTime FechaServidor = DateTime.Now;
                        DateTime FechaNULL = new DateTime(1900, 1, 1, 0, 0, 0, FechaServidor.Kind);

                        if (eEmpleado.fecha_ingreso != FechaNULL)
                        {
                            //fecha ingreso mayor a la fecha actual, TODAVIA NO MARCA
                            if (eEmpleado.fecha_ingreso > FechaServidor)
                                registrarMarcacion = false;
                        }

                        if (eEmpleado.fecha_salida != FechaNULL)
                        {
                            //fecha del servidor es mayor a la fecha de retiro del empleado (YA NO DEBE MARCAR)
                            DateTime fechaSalidaEmpleado = eEmpleado.fecha_salida.Value.AddDays(1);
                            if (FechaServidor > fechaSalidaEmpleado)
                                registrarMarcacion = false;
                        }

                        if (registrarMarcacion)
                        {
                            Marcacion bMarcacion = new Marcacion(this._DBcontext);
                            DateTime fechaHOY = DateTime.Now;
                            foreach (EPU.Turno eTurno in eEmpleado.listaTurno.Where(x => x.entrada.DayOfWeek == fechaHOY.DayOfWeek))
                            {
                                EPU.Marcacion eMarcacionXDefecto = new EPU.Marcacion();

                                DateTime fechaEntradaProgramada = new DateTime(fechaHOY.Year, fechaHOY.Month, fechaHOY.Day,
                                                                                eTurno.entrada.Hour, eTurno.entrada.Minute, eTurno.entrada.Second, fechaHOY.Kind);

                                DateTime fechaSalidaProgramada = new DateTime(fechaHOY.Year, fechaHOY.Month, fechaHOY.Day,
                                                                            eTurno.salida.Hour, eTurno.salida.Minute, eTurno.salida.Second, fechaHOY.Kind);

                                eMarcacionXDefecto.id_empleado = eEmpleado.id;
                                eMarcacionXDefecto.codigo_turno = eTurno.codigo_turno;
                                eMarcacionXDefecto.entrada_programada = fechaEntradaProgramada; //entrada programada
                                eMarcacionXDefecto.salida_programada = fechaSalidaProgramada; //salida programada
                                eMarcacionXDefecto.latitud_entrada = 0D;
                                eMarcacionXDefecto.longitud_entrada = 0D;
                                eMarcacionXDefecto.latitud_salida = 0D;
                                eMarcacionXDefecto.longitud_salida = 0D;

                                EPU.Marcacion eAux = new Marcacion(this._DBcontext).BuscarMarcacion(eMarcacionXDefecto);
                                if (eAux == null)
                                {
                                    this._DBcontext.Marcacion.Add(eMarcacionXDefecto);
                                    this._DBcontext.SaveChanges();
                                }
                            }
                        }
                    }

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


        public void ValidarDatosEmpleado(EPU.Empleado empleado, bool esNuevo)
        {
            try
            {
                // Validar código
                if (!string.IsNullOrEmpty(empleado.codigo) && _DBcontext.Empleado.Any(e => e.codigo.Trim() == empleado.codigo.Trim() && (esNuevo || e.id != empleado.id)))
                {
                    throw new Exception($"Ya existe un empleado con el código '{empleado.codigo}'.");
                }

                // Validar usuario
                if (_DBcontext.Empleado.Any(e => e.usuario.Trim() == empleado.usuario.Trim() && (esNuevo || e.id != empleado.id)))
                {
                    throw new Exception($"Ya existe un empleado con el usuario '{empleado.usuario}'.");
                }

                // Validar correo
                if (!string.IsNullOrEmpty(empleado.correo) && _DBcontext.Empleado.Any(e => e.correo.Trim() == empleado.correo.Trim() && (esNuevo || e.id != empleado.id)))
                {
                    throw new Exception($"Ya existe un empleado con el correo '{empleado.correo}'.");
                }

                // Validar teléfono
                if (!string.IsNullOrEmpty(empleado.telefono) && _DBcontext.Empleado.Any(e => e.telefono.Trim() == empleado.telefono.Trim() && (esNuevo || e.id != empleado.id)))
                {
                    throw new Exception($"Ya existe un empleado con el teléfono '{empleado.telefono}'.");
                }
            }
            catch (Exception ex)
            {
                // Manejar el error o relanzarlo
                throw ex;
            }
        }


        public bool ModificarEmpleadoModoIndependiente(EPU.Empleado eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    ValidarDatosEmpleado(eEntidad, false);

                    var eEmpleadoAux = _DBcontext.Empleado.AsNoTracking().FirstOrDefault(p => p.estado == 0 && p.id == eEntidad.id);
                    if (eEmpleadoAux != null)
                    {
                        // Revisar contraseña
                        eEntidad.clave = eEmpleadoAux.clave;

                        // Revisar jornada
                        if (eEntidad.id_jornada != eEmpleadoAux.id_jornada)
                        {
                            //eliminar turnos anteriores
                            this._DBcontext.Turno.RemoveRange(this._DBcontext.Turno.Where(e => e.id_empleado == eEntidad.id));
                            this._DBcontext.SaveChanges();

                            var detalles_jornada = this._DBcontext.Detalle_Jornada.Where(d => d.id_jornada == eEntidad.id_jornada).ToList();

                            BSE.Clasificador bClasificador = new BSE.Clasificador(this._DBcontext);
                            const long tipoDiaSemana = 4;
                            List<ESE.Clasificador> dias = bClasificador.ListarPorTipo(tipoDiaSemana);

                            // Nueva lista turnos, para los nuevos turnos a crearse
                            eEntidad.listaTurno = new List<EPU.Turno>();

                            foreach (EPU.Detalle_Jornada detalle in detalles_jornada)
                            {
                                EPU.Turno turno = new EPU.Turno()
                                {
                                    id_empleado = eEntidad.id,
                                    codigo_turno = (int)detalle.id,
                                    entrada = detalle.hora_entrada,
                                    salida = detalle.hora_salida,
                                    descripcion_turno = $"Turno {detalle.id} - {dias.Find(d => Convert.ToInt32(d.valor) == detalle.dia).descripcion} ({detalle.hora_entrada.ToString("HH:mm")} - {detalle.hora_salida.ToString("HH:mm")})",
                                    anticipo_entrada = (int)((detalle.hora_entrada - detalle.inicio_marcacion_entrada).TotalHours * 60),
                                    tolerancia_entrada = (int)((detalle.fin_marcacion_entrada - detalle.hora_entrada).TotalHours * 60),
                                    anticipo_salida = (int)((detalle.hora_salida - detalle.inicio_marcacion_salida).TotalHours * 60),
                                    tolerancia_salida = (int)((detalle.fin_marcacion_salida - detalle.hora_salida).TotalHours * 60),
                                    idc_dia = detalle.dia
                                };

                                this._DBcontext.Turno.Add(turno);
                                this._DBcontext.SaveChanges();
                            }
                        }
                    }

                    this._DBcontext.Empleado.Update(eEntidad);
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

        public List<EPU.Empleado> GetListaModoIndependiente()
        {
            try
            {
                var listaEmpleado = _DBcontext.Empleado.Where(u => u.estado == 0).ToList();
                if (listaEmpleado != null && listaEmpleado.Any())
                {
                    listaEmpleado = listaEmpleado.Select(e =>
                    {
                        e.nombre = e.nombre + " " + e.apellido_paterno + " " + e.apellido_materno;
                        e.listaTurno = this._DBcontext.Turno.Where(t => t.id_empleado == e.id).ToList();
                        e.jornada = this._DBcontext.Jornada.FirstOrDefault(j => j.id == e.id_jornada);
                        e.sucursal = this._DBcontext.Sucursal.FirstOrDefault(s => s.id == e.id_sucursal);
                        return e;
                    }).ToList();
                }

                return listaEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw;
            }
        }


        public List<Foto_Empleado> ObtenerFotos(long id_empleado, int tipo = 0)
        {
            try
            {
                var query = this._DBcontext.Foto_Empleado.Where(e => e.id_empleado == id_empleado);

                if (tipo != 0)
                {
                    query = query.Where(e => e.tipo_foto_empleado == (TipoFotoEmpleado)tipo);
                }

                var listaFotos = query.ToList();
                return listaFotos;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public bool GuardarFotosV2(long id_empleado, List<Entities.Public.Foto_Empleado> listaFoto, int tipo)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    var listaEliminar = this._DBcontext.Foto_Empleado.Where(e => e.id_empleado == id_empleado && e.tipo_foto_empleado == (TipoFotoEmpleado)tipo).ToList();
                    this._DBcontext.Foto_Empleado.RemoveRange(listaEliminar);
                    this._DBcontext.SaveChanges();

                    this._DBcontext.Foto_Empleado.AddRange(listaFoto);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    throw;
                }
            }

        }

        public EPU.Empleado LoginV2(string usuario, string contrasena)
        {
            try
            {
                EPU.Empleado eEmpleado = _DBcontext.Empleado.FirstOrDefault(u => u.usuario.ToUpper() == usuario.ToUpper() && u.clave == contrasena);
                if (eEmpleado != null)
                {
                    eEmpleado.listaFotoEmpleado = ObtenerFotos(eEmpleado.id, (int)TipoFotoEmpleado.Perfil);
                }
                return eEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        #endregion

    }
}