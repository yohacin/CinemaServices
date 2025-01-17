using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESE = Entities.Security;

namespace Business.Security
{
    public class Clasificador : Base
    {
        public Clasificador(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public List<ESE.Clasificador> ListarPorTipo(long idTipoClasificador)
        {
            try
            {
                return this._DBcontext.ClasificadorSecuriry.Where(c => c.id_tipo_clasificador == idTipoClasificador).OrderBy(c => c.valor).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}