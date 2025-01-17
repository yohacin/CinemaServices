using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business
{
    public interface IBusiness<T>
    {

        bool Guardar(T eEntidad);

        bool Modificar(T eEntidad);

        bool Eliminar(long id);

        #region metodos get, listado, etc
        T GetEntity(long id);

        List<T> GetLista();

        #endregion
    }
}
