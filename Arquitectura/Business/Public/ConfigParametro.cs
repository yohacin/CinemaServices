using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using EPU = Entities.Public;

namespace Business.Public
{
    public class ConfigParametro : Base, IBusiness<EPU.ConfigParametro>
    {
        public ConfigParametro(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Eliminar(long id)
        {
            throw new NotImplementedException();
        }

        public EPU.ConfigParametro GetEntity(long id)
        {
            throw new NotImplementedException();
        }

        public List<EPU.ConfigParametro> GetLista()
        {
            throw new NotImplementedException();
        }

        public bool Guardar(EPU.ConfigParametro eEntidad)
        {
            throw new NotImplementedException();
        }

        public bool Modificar(EPU.ConfigParametro eEntidad)
        {
            throw new NotImplementedException();
        }

        public List<EPU.ConfigParametro> ListarParametros()
        {
            try
            {
                return _DBcontext.ConfigParametro.OrderBy(c => c.id).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.ConfigParametro> ListarByImei(string pImei)
        {
            try
            {
                string query = $@"SELECT cp.id, cp.nombre, cp.descripcion,
                                        CASE WHEN cpm.valor NOTNULL THEN cpm.valor ELSE cp.valor END as valor
                                  FROM public.movil m
                                        INNER JOIN public.config_parametro_movil cpm ON m.id=cpm.id_movil AND m.numero_imei='{pImei}'
                                        RIGHT JOIN public.config_parametro cp ON cpm.id_config_parametro=cp.id";

                return _DBcontext.ConfigParametro.FromSqlRaw(query).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.ConfigParametro> ListarByEmpleado(long id_empleado)
        {
            try
            {
                string query = $@"SELECT cp.ID, cp.nombre, cp.descripcion, CASE WHEN cpe.valor NOTNULL THEN
																						cpe.valor ELSE cp.valor
																				 END AS valor
                                FROM PUBLIC.config_parametro cp
                                LEFT JOIN PUBLIC.config_parametro_empleado cpe ON cp.Id = cpe.id_config_parametro and cpe.id_empleado = {id_empleado}
                ";

                return _DBcontext.ConfigParametro.FromSqlRaw(query).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.ConfigParametro BuscarParametroById(long pId)
        {
            try
            {
                string query = $@"select * from public.config_parametro where id={pId}";

                return _DBcontext.ConfigParametro.FromSqlRaw(query).First();
                // return _DBcontext.ConfigParametro.FirstOrDefault(c => c.id == pId);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.ConfigParametro BuscarParametroByNombre(string sNombre, long id_empleado = 0)
        {
            try
            {
                string query = $@"SELECT con.id, con.nombre, CASE WHEN con_emp.valor IS NOT NULL THEN
		                                con_emp.valor ELSE con.valor
	                                    END, con.descripcion
                                  FROM PUBLIC.config_parametro con
                                  LEFT JOIN config_parametro_empleado con_emp ON con.ID = con_emp.id_config_parametro  AND con_emp.id_empleado = {id_empleado}
                                  WHERE con.nombre = '{sNombre}' AND con.estado";

                return _DBcontext.ConfigParametro.FromSqlRaw(query).First();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool ValidarParametroByNombre(string sNombre, string sValor, bool valorPorDefecto, long id_empleado = 0)
        {
            var config = BuscarParametroByNombre(sNombre, id_empleado);
            if (config != null)
            {
                return config.valor.ToUpper() == sValor.ToUpper();
            }
            return valorPorDefecto;
        }
    }
}