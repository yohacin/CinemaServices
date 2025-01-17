using Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Business
{
    public class Base : IBase
    {
        protected Data.ApplicationDbContext _DBcontext;

        /// <summary>
        /// Constructor Generico para todas las clases de Negocio
        /// Inicia el DBcontext para Trabajar con Base de datos
        /// Inicia el Log4net para Loggear dentro de la clase
        /// </summary>
        public Base(IApplicationDbContext ApplicationDbContext)
        {
            _DBcontext = (ApplicationDbContext)ApplicationDbContext; // Data.Factory.GetDbContext;
        }

        /// <summary>
        /// Retorna la Transaccion Actual para Trabajar con BD
        /// trabaja con el Factory para Iniciar la Transaccion o devolver la Actual
        /// </summary>
        /// <returns> Retorna un iDbContextTransaction </returns>
        public async Task<IDbContextTransaction> _GetTransactionAsync()
        {
            return await this._DBcontext.BeginTransactionAsync();
        }

        //private  IDbContextTransaction  _Current_Transaction;
        //protected  log4net.ILog _log;

        ///// <summary>
        ///// Constructor Generico para todas las clases de Negocio
        ///// Inicia el DBcontext para Trabajar con Base de datos
        ///// Inicia el Log4net para Loggear dentro de la clase
        ///// </summary>
        //protected Base()
        //{
        // //_log = log4net.LogManager.GetLogger(this.GetType());
        //_DBcontext = Data.Factory.GetDbContext;
        //}

        ///// <summary>
        ///// Retorna la Transaccion Actual para Trabajar con BD
        ///// trabaja con el Factory para Iniciar la Transaccion o devolver la Actual
        ///// </summary>
        ///// <returns> Retorna un IDbContextTransaction </returns>
        //protected Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction  _GetTransaction()
        //{
        //    return  (this._Current_Transaction != null)?
        //              _Current_Transaction : _DBcontext.Database.BeginTransaction();
        //}

        //public void _UndoingChangesDbContextLevel()
        //{
        //    foreach (EntityEntry entry in this._DBcontext.ChangeTracker.Entries())
        //    {
        //        switch (entry.State)
        //        {
        //            case EntityState.Modified:
        //                entry.State = EntityState.Unchanged;
        //                break;
        //            case EntityState.Added:
        //                entry.State = EntityState.Detached;
        //                break;
        //            case EntityState.Deleted:
        //                entry.Reload();
        //                break;
        //            default: break;
        //        }
        //    }
        //}

        //public string GenKeys(List<Object> keys)
        //{
        //    if (keys.Count > 0)
        //    {
        //        String filter = "(";
        //        String coma = "";
        //        foreach (Object iter in keys)
        //        {
        //            filter += coma + iter.ToString();
        //            coma = ",";
        //        }
        //        filter += ")";

        //        return filter;
        //    }

        //    return "(0)";

        //}
    }
}