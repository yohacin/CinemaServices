using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Empresa : Base, IBusiness<EPU.Empresa>
    {
        public Empresa(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Empresa eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Empresa.Add(eEntidad);
                    this._DBcontext.SaveChanges();

                    eEntidad.listaEmpleado_Empresa = eEntidad.listaEmpleado_Empresa.Select(e =>
                    {
                        e.id_empresa = eEntidad.id;
                        return e;
                    }).ToList();
                    foreach (var item in eEntidad.listaEmpleado_Empresa)
                    {
                        this._DBcontext.Empleado_Empresa.Add(item);
                    }
                    //this._DBcontext.Empleado_Empresa.AddRange(eEntidad.listaEmpleado_Empresa);
                    this._DBcontext.SaveChanges();

                    if (eEntidad.listFotos != null)
                    {
                        if (eEntidad.listFotos.Count > 0)
                        {
                            eEntidad.listFotos[0].id_empresa = eEntidad.id;
                            eEntidad.listFotos[0].posicion = 1;
                            this._DBcontext.Foto_Empresa.Add(eEntidad.listFotos[0]);
                            this._DBcontext.SaveChanges();
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

        public bool GuardarSinEmpleado(EPU.Empresa eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Empresa.Add(eEntidad);
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

        public bool Modificar(EPU.Empresa eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Empresa.Update(eEntidad);
                    this._DBcontext.SaveChanges();

                    //eliminar accesos anteriores
                    this._DBcontext.Empleado_Empresa.RemoveRange(this._DBcontext.Empleado_Empresa.Where(e => e.id_empresa == eEntidad.id));
                    this._DBcontext.SaveChanges();

                    eEntidad.listaEmpleado_Empresa = eEntidad.listaEmpleado_Empresa.Select(e =>
                    {
                        e.id_empresa = eEntidad.id;
                        return e;
                    }).ToList();
                    this._DBcontext.Empleado_Empresa.AddRange(eEntidad.listaEmpleado_Empresa);
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

        public bool ModificarSinEmpleado(EPU.Empresa eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Empresa.Update(eEntidad);
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

        public bool GuardarFotoEmpresa(EPU.Foto_Empresa eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Foto_Empresa.Add(eEntidad);
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
                    EPU.Empresa eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Empresa Inexistente!!...");

                    var listaEmpresaEngagement = this._DBcontext.Engagement.Where(e => e.estado == 0 && e.estado_ejecucion != 3).ToList();
                    if (listaEmpresaEngagement != null)
                        if (listaEmpresaEngagement.Count > 0) throw new Exception("La empresa tiene asignada engagement en ejecución");

                    this._DBcontext.Empresa.Remove(eEntidad);
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

        public EPU.Empresa GetEntity(long id)
        {
            try
            {
                var eEmpresa = _DBcontext.Empresa.FirstOrDefault(p => p.id == id);
                if (eEmpresa != null)
                {
                    eEmpresa.listFotos = this._DBcontext.Foto_Empresa.Where(e => e.id_empresa == eEmpresa.id).OrderByDescending(e => e.id).ToList();
                    eEmpresa.listaEmpleado_Empresa = this._DBcontext.Empleado_Empresa.Where(e => e.id_empresa == eEmpresa.id).ToList();
                    eEmpresa.listaEmpleado = new List<EPU.Empleado>();
                    foreach (var item in eEmpresa.listaEmpleado_Empresa)
                    {
                        var eEmpleado = this._DBcontext.Empleado.FirstOrDefault(e => e.id == item.id_empleado);
                        eEmpresa.listaEmpleado.Add(eEmpleado);
                    }
                }

                return eEmpresa;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public string GetEntityNombre(long id)
        {
            try
            {
                var eEmpresa = _DBcontext.Empresa.FirstOrDefault(p => p.id == id);

                return (eEmpresa == null) ? "" : eEmpresa.nombre;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empresa> GetLista()
        {
            try
            {
                return _DBcontext.Empresa.Where(e => e.marca_eliminado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empresa> GetListaByEmpleado(long id_empleado)
        {
            try
            {
                string sQuery = $@"
                    SELECT DISTINCT em.*
                    FROM public.empresa em
                    JOIN public.empleado_empresa ee on ee.id_empresa = em.id and ee.id_empleado = {id_empleado}
                    WHERE ee.fecha_fin is null
                ";
                return _DBcontext.Empresa.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empresa> ListarEmpresasUsuario(long id_empleado)
        {
            try
            {
                string sQuery = $@" SELECT em.*
                                     FROM public.empleado emp
                                     JOIN public.empleado_empresa empr on empr.id_empleado=emp.id and empr.fecha_fin is null
                                     JOIN public.empresa em on em.id=empr.id_empresa and em.tiene_sucursales=false
                                     WHERE emp.id={id_empleado} ORDER BY em.nombre;";

                List<EPU.Empresa> listaEmpresas = _DBcontext.Empresa.FromSqlRaw(sQuery).ToList();
                if (listaEmpresas != null)
                {
                    listaEmpresas = listaEmpresas.Select(emp =>
                    {
                        emp.listFotos = new List<EPU.Foto_Empresa>();
                        var eFoto_EmpresaUltima = this._DBcontext.Foto_Empresa.Where(f => f.id_empresa == emp.id).OrderByDescending(f => f.id).FirstOrDefault();
                        if (eFoto_EmpresaUltima != null)
                        {
                            emp.listFotos.Add(eFoto_EmpresaUltima);
                        }

                        return emp;
                    }).ToList();
                }

                return listaEmpresas;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empresa> ListarCasaMatrices()
        {
            try
            {
                return _DBcontext.Empresa.Where(e => e.tiene_sucursales == true).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empresa> GetListaByEngagementEmpleado(long id_empleado)
        {
            try
            {
                string sQuery = $@"
                    SELECT DISTINCT em.*
                    FROM public.empresa em
                    JOIN public.engagement eng on eng.id_empresa = em.id
                    JOIN PUBLIC.engagement_empleado eng_emp on eng_emp.id_engagement = eng.id
                    WHERE eng.estado = 0 and eng.estado_ejecucion <> 3 and eng_emp.id_empleado = {id_empleado}
                ";
                return _DBcontext.Empresa.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }


        public EPU.Empresa GetEntityModoIndependiente(long id)
        {
            try
            {
                var eEmpresa = _DBcontext.Empresa.FirstOrDefault(p => p.id == id);
                if (eEmpresa != null)
                {
                    eEmpresa.listFotos = this._DBcontext.Foto_Empresa.Where(e => e.id_empresa == eEmpresa.id).OrderByDescending(e => e.id).ToList();
                    eEmpresa.listaEmpleado_Empresa = this._DBcontext.Empleado_Empresa.Where(e => e.id_empresa == eEmpresa.id).ToList();
                    eEmpresa.listaEmpleado = new List<EPU.Empleado>();
                    foreach (var item in eEmpresa.listaEmpleado_Empresa)
                    {
                        var eEmpleado = this._DBcontext.Empleado.FirstOrDefault(e => e.id == item.id_empleado && e.estado == 0);
                        if (eEmpleado != null) eEmpresa.listaEmpleado.Add(eEmpleado);
                    }
                }

                return eEmpresa;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}