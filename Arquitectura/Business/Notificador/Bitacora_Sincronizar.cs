using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using ENO = Entities.Notificador;

namespace Business.Notificador
{
    public class Bitacora_Sincronizar : Base
    {
        public Bitacora_Sincronizar(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public long CrearConfig(ENO.Bitacora_Sincronizar eBitacora_Sincronizar)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Bitacora_Sincronizar.Add(eBitacora_Sincronizar);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return eBitacora_Sincronizar.id;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public bool ModificarConfig(ENO.Bitacora_Sincronizar eBitacora_Sincronizar)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Bitacora_Sincronizar.Update(eBitacora_Sincronizar);
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

        public ENO.Bitacora_Sincronizar GetUltimaBitacora()
        {
            try
            {
                return this._DBcontext.Bitacora_Sincronizar.OrderByDescending(b => b.id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Bitacora_Sincronizar> ListBitacora()
        {
            try
            {
                List<ENO.Bitacora_Sincronizar> lstBitacora = this._DBcontext.Bitacora_Sincronizar.FromSqlRaw(@"SELECT bs.*
                                                                FROM notificador.bitacora_sincronizar bs
                                                                WHERE bs.metodo LIKE '%Sincronizar Imagen%'
                                                                ORDER BY bs.id ASC"
                                                             ).ToList<ENO.Bitacora_Sincronizar>();
                return lstBitacora;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Bitacora_Sincronizar> ListBitacoraDetalle()
        {
            try
            {
                List<ENO.Bitacora_Sincronizar> lstBitacoraAux = this._DBcontext.Bitacora_Sincronizar.FromSqlRaw(@"SELECT bs.*
                                                                                                            FROM notificador.bitacora_sincronizar bs
                                                                                                            WHERE bs.metodo = 'Sincronizar'
                                                                                                            ORDER BY bs.id DESC
                                                                                                            LIMIT 3"
                                                                                                          ).ToList<ENO.Bitacora_Sincronizar>();
                foreach (ENO.Bitacora_Sincronizar oBitacora in lstBitacoraAux)
                {
                    oBitacora.lstBitacora = new List<ENO.Bitacora_Sincronizar>();
                    oBitacora.lstBitacora = ListBitacoraByIdAgrup(oBitacora.id);

                    int cantMetodo = oBitacora.lstBitacora.Count();

                    if (cantMetodo == 0)
                    {
                        oBitacora.cant_metodos = 1;
                        oBitacora.fecha_fin = oBitacora.fecha;
                    }
                    else
                    {
                        oBitacora.cant_metodos = cantMetodo + 1;
                        oBitacora.fecha_fin = oBitacora.lstBitacora[cantMetodo - 1].fecha;
                    }

                    foreach (ENO.Bitacora_Sincronizar oBitacoraH in oBitacora.lstBitacora)
                    {
                        if (!String.IsNullOrEmpty(oBitacoraH.contenido))
                        {
                            oBitacora.contenido = oBitacoraH.contenido;
                        }
                        if (!oBitacoraH.transaccion)
                        {
                            oBitacora.transaccion = false;
                        }
                    }
                }

                return lstBitacoraAux;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Bitacora_Sincronizar> ListBitacoraByIdAgrup(long idAgrup)
        {
            try
            {
                List<ENO.Bitacora_Sincronizar> lstBitacora = this._DBcontext.Bitacora_Sincronizar.FromSqlRaw($@"SELECT bs.*
                                                                                                             FROM notificador.bitacora_sincronizar bs
                                                                                                             WHERE bs.id_agrupador = {idAgrup}"
                                                                                                          ).ToList<ENO.Bitacora_Sincronizar>();
                return lstBitacora;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public IQueryable<ENO.Bitacora_Sincronizar> GetListaBitacora()
        {
            try
            {
                var lstBitacoraAux = this._DBcontext.Bitacora_Sincronizar.FromSqlRaw(@"SELECT bs.*
                                                                                    FROM notificador.bitacora_sincronizar bs
                                                                                    WHERE bs.metodo = 'Sincronizar'
                                                                                    ORDER BY bs.id DESC "
                                                                                  );
                foreach (ENO.Bitacora_Sincronizar oBitacora in lstBitacoraAux)
                {
                    oBitacora.lstBitacora = new List<ENO.Bitacora_Sincronizar>();
                    oBitacora.lstBitacora = ListBitacoraByIdAgrup(oBitacora.id);

                    int cantMetodo = oBitacora.lstBitacora.Count();

                    if (cantMetodo == 0)
                    {
                        oBitacora.cant_metodos = 1;
                        oBitacora.fecha_fin = oBitacora.fecha;
                    }
                    else
                    {
                        oBitacora.cant_metodos = cantMetodo + 1;
                        oBitacora.fecha_fin = oBitacora.lstBitacora[cantMetodo - 1].fecha;
                    }

                    foreach (ENO.Bitacora_Sincronizar oBitacoraH in oBitacora.lstBitacora)
                    {
                        if (!String.IsNullOrEmpty(oBitacoraH.contenido) && oBitacoraH.contenido.Contains("sJson"))
                        {
                            oBitacora.contenido = oBitacoraH.contenido;
                        }
                        if (!oBitacoraH.transaccion)
                        {
                            oBitacora.transaccion = false;
                        }
                    }
                }

                return lstBitacoraAux;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}