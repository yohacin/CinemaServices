using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Abreviaturas

using ENO = Entities.Notificador;

#endregion Abreviaturas

namespace Business.Notificador
{
    public class Clasificador : Base
    {
        public Clasificador(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public List<ENO.Clasificador> ListarByTipo(long id_tipo_clasificador)
        {
            try
            {
                return this._DBcontext.Clasificador.Where(c => c.id_tipo_clasificador == id_tipo_clasificador).OrderBy(c => c.valor_referencial).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}