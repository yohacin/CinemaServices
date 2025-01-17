using Data;
using Entities.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using EPU = Entities.Public;
using BPU = Business.Public;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Business.Public;

public class Marcacion_Biometrico : Base, IBusiness<EPU.Marcacion_Biometrico>
{
    public Marcacion_Biometrico(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
    {
    }


    public EPU.Marcacion_Biometrico GetEntity(long id)
    {
        try
        {
            EPU.Marcacion_Biometrico eMarcacion_Biometrico = _DBcontext.Marcacion_Biometrico.FirstOrDefault(p => p.id == id);
            if (eMarcacion_Biometrico == null) return null;
            eMarcacion_Biometrico.listaDetalleMarcacion = _DBcontext.Detalle_Marcacion_Biometrico.Where(x => x.id_marcacion_biometrico == eMarcacion_Biometrico.id).ToList();
            eMarcacion_Biometrico.usuario = _DBcontext.Usuario.FirstOrDefault(u => u.id == eMarcacion_Biometrico.id_usuario);
            eMarcacion_Biometrico.empresa = _DBcontext.Empresa.FirstOrDefault(u => u.id == eMarcacion_Biometrico.id_empresa);
            eMarcacion_Biometrico.formato = _DBcontext.Formato_Marcacion_Biometrico.FirstOrDefault(u => u.id == eMarcacion_Biometrico.id_formato_marcacion_biometrico);

            return eMarcacion_Biometrico;
        }
        catch (Exception)
        {
            //this._log.Error(ex);
            throw;
        }
    }

    public List<EPU.Marcacion_Biometrico> GetLista()
    {
        try
        {
            var listaMarcaciones = _DBcontext.Marcacion_Biometrico.ToList();

            if (listaMarcaciones != null && listaMarcaciones.Any())
            {
                listaMarcaciones = listaMarcaciones.Select( m => {
                    m.usuario = this._DBcontext.Usuario.FirstOrDefault(j => j.id == m.id_usuario);
                    m.empresa = this._DBcontext.Empresa.FirstOrDefault(j => j.id == m.id_empresa);
                    return m;
                }).ToList(); ;
            }

            return listaMarcaciones;
        }
        catch (Exception)
        {
            //this._log.Error(ex);
            throw;
        }
    }

    public bool Guardar(EPU.Marcacion_Biometrico eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Marcacion_Biometrico.Add(eEntidad);
                this._DBcontext.SaveChanges();

                // lista detalle Marcacion_Biometrico
                eEntidad.listaDetalleMarcacion = eEntidad.listaDetalleMarcacion.Select(e =>
                {
                    e.id = 0;
                    e.id_marcacion_biometrico = eEntidad.id;
                    return e;
                }).ToList();

                this._DBcontext.Detalle_Marcacion_Biometrico.AddRange(eEntidad.listaDetalleMarcacion);
                this._DBcontext.SaveChanges();

                oTrans.Commit();
                return true;
            }
            catch (Exception)
            {
                oTrans.Rollback();
                //this._log.Error(ex);
                throw;
            }
        }
    }

    public bool Modificar(EPU.Marcacion_Biometrico eEntidad)
    {
        return false;
    }

    public bool Eliminar(long id)
    {
        return false;
    }

    public bool GuardarMarcaciones(EPU.Marcacion_Biometrico eEntidad, int tolerancia_fija)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Marcacion_Biometrico.Add(eEntidad);
                this._DBcontext.SaveChanges();

                // lista detalle Marcacion_Biometrico
                eEntidad.listaDetalleMarcacion = eEntidad.listaDetalleMarcacion.Select(e =>
                {
                    e.id = 0;
                    e.id_marcacion_biometrico = eEntidad.id;
                    return e;
                }).ToList();

                this._DBcontext.Detalle_Marcacion_Biometrico.AddRange(eEntidad.listaDetalleMarcacion);
                this._DBcontext.SaveChanges();



                // GUARDAR LAS MARCACIONES
                List<EPU.Marcacion> marcacionesActualizadas = new List<EPU.Marcacion>();

                foreach (var detalleMarcacionBio in eEntidad.listaDetalleMarcacion)
                {
                    // Paso 1: Buscar el empleado por código biométrico
                    var empleado = _DBcontext.Empleado
                        .FirstOrDefault(e => e.codigo == detalleMarcacionBio.codigo_biometrico);

                    if (empleado == null)
                    {
                        Console.WriteLine($"No se encontró un empleado con el código biométrico: {detalleMarcacionBio.codigo_biometrico}");
                        continue;
                    }

                    // Paso 2: Buscar las marcaciones del día para este empleado
                    var marcaciones = ObtenerMarcacionesPorUsuarioYFecha(empleado.id, detalleMarcacionBio.fecha_marcacion);

                    if (!marcaciones.Any())
                    {
                        Console.WriteLine($"No se encontraron marcaciones programadas para el empleado ID: {empleado.id} en la fecha: {detalleMarcacionBio.fecha_marcacion.Date}");
                        continue;
                    }

                    // Paso 3: Buscar la marcación más cercana a la fecha del biométrico
                    EPU.Marcacion marcacionCercana = null;
                    bool esEntrada = false;
                    double menorDiferencia = double.MaxValue;

                    foreach (var marcacion in marcaciones)
                    {
                        var diferenciaEntrada = Math.Abs((marcacion.entrada_programada - detalleMarcacionBio.fecha_marcacion).TotalMinutes);
                        var diferenciaSalida = Math.Abs((marcacion.salida_programada - detalleMarcacionBio.fecha_marcacion).TotalMinutes);

                        if (diferenciaEntrada < menorDiferencia)
                        {
                            menorDiferencia = diferenciaEntrada;
                            esEntrada = true;
                            marcacionCercana = marcacion;
                        }

                        if (diferenciaSalida < menorDiferencia)
                        {
                            menorDiferencia = diferenciaSalida;
                            esEntrada = false;
                            marcacionCercana = marcacion;
                        }
                    }

                    // Si no hay una marcación cercana, no continuar
                    if (marcacionCercana == null)
                    {
                        Console.WriteLine($"No se encontró una marcación cercana para el empleado ID: {empleado.id}");
                        continue;
                    }

                    // Paso 4: Verificar si la marcación tiene un turno asociado
                    var turno = _DBcontext.Turno
                        .FirstOrDefault(t => t.id_empleado == empleado.id &&
                                             t.codigo_turno == marcacionCercana.codigo_turno);

                    // Ajustar los rangos con tolerancias
                    DateTime inicioRango, finRango;

                    if (turno != null)
                    {
                        if (esEntrada)
                        {
                            inicioRango = marcacionCercana.entrada_programada.AddMinutes(-turno.anticipo_entrada);
                            finRango = marcacionCercana.entrada_programada.AddMinutes(turno.tolerancia_entrada);
                        }
                        else
                        {
                            inicioRango = marcacionCercana.salida_programada.AddMinutes(-turno.anticipo_salida);
                            finRango = marcacionCercana.salida_programada.AddMinutes(turno.tolerancia_salida);
                        }
                    }
                    else
                    {
                        // Usar tolerancia fija 
                        if (esEntrada)
                        {
                            inicioRango = marcacionCercana.entrada_programada.AddMinutes(-tolerancia_fija);
                            finRango = marcacionCercana.entrada_programada.AddMinutes(tolerancia_fija);
                        }
                        else
                        {
                            inicioRango = marcacionCercana.salida_programada.AddMinutes(-tolerancia_fija);
                            finRango = marcacionCercana.salida_programada.AddMinutes(tolerancia_fija);
                        }
                    }

                    // Verificar si está dentro del rango
                    if (detalleMarcacionBio.fecha_marcacion >= inicioRango && detalleMarcacionBio.fecha_marcacion <= finRango)
                    {
                        // Verificar si la marcacionCercana ya está en la lista de marcacionesActualizadas
                        var marcacionExistente = marcacionesActualizadas.FirstOrDefault(m => m.id == marcacionCercana.id);

                        if (marcacionExistente != null)
                        {
                            // Si ya existe, solo actualizamos los campos correspondientes
                            if (esEntrada)
                            {
                                marcacionExistente.entrada_real = detalleMarcacionBio.fecha_marcacion;
                                marcacionExistente.id_empresa_entrada = detalleMarcacionBio.id_empresa;
                                marcacionExistente.tipo_marcacion_entrada = "BIOMETRICO";
                            }
                            else
                            {
                                marcacionExistente.salida_real = detalleMarcacionBio.fecha_marcacion;
                                marcacionExistente.id_empresa_salida = detalleMarcacionBio.id_empresa;
                                marcacionExistente.tipo_marcacion_salida = "BIOMETRICO";
                            }
                        }
                        else
                        {
                            // Si no está en la lista, agregar la marcación con los campos correspondientes
                            if (esEntrada)
                            {
                                marcacionCercana.entrada_real = detalleMarcacionBio.fecha_marcacion;
                                marcacionCercana.id_empresa_entrada = detalleMarcacionBio.id_empresa;
                                marcacionCercana.tipo_marcacion_entrada = "BIOMETRICO";
                            }
                            else
                            {
                                marcacionCercana.salida_real = detalleMarcacionBio.fecha_marcacion;
                                marcacionCercana.id_empresa_salida = detalleMarcacionBio.id_empresa;
                                marcacionCercana.tipo_marcacion_salida = "BIOMETRICO";
                            }

                            // Agregar la marcación actualizada a la lista
                            marcacionesActualizadas.Add(marcacionCercana);
                        }
                    }
                    else
                    {
                        // La fecha del biométrico no cumple con los parámetros
                        Console.WriteLine($"La fecha de marcación biométrica no cumple con los parámetros para registrarse. Empleado ID: {empleado.id}, Fecha: {detalleMarcacionBio.fecha_marcacion}");
                    }
                }

                if (marcacionesActualizadas.Any())
                {
                    this._DBcontext.Marcacion.UpdateRange(marcacionesActualizadas);
                    this._DBcontext.SaveChanges();
                }

                oTrans.Commit();
                return true;
            }
            catch (Exception)
            {
                oTrans.Rollback();
                //this._log.Error(ex);
                throw;
            }
        }
    }

    public List<EPU.Marcacion> ObtenerMarcacionesPorUsuarioYFecha(long empleado_id, DateTime fecha_marcacion)
    {
        try
        {
            var query = @"
                            SELECT * 
                            FROM public.marcacion
                            WHERE id_empleado = @idEmpleado
                            AND entrada_programada::DATE = @fechaMarcacion
                            AND (entrada_real IS NULL OR salida_real IS NULL)
                            ";

            var marcaciones = _DBcontext.Marcacion.FromSqlRaw(query,
                new NpgsqlParameter("@idEmpleado", empleado_id),
                new NpgsqlParameter("@fechaMarcacion", fecha_marcacion.Date)
            ).AsNoTracking().ToList();

            return marcaciones;
        }
        catch (Exception)
        {
            //this._log.Error(ex);
            throw;
        }

    }

}

