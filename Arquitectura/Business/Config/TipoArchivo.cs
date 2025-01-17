using Data;
using System;
using System.Collections.Generic;
using System.Linq;

#region Abreviaturas

using ECO = Entities.Config;

#endregion Abreviaturas

namespace Business.Config
{
    public class TipoArchivo : Base
    {
        public TipoArchivo(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public List<ECO.Tipo_Archivo> GetListaTipoArchivo()
        {
            try
            {
                return _DBcontext.Tipo_Archivo.ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}