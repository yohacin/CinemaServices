// Librerias locales
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public sealed class Factory
    {
        public static string CurrentCnx { get; set; }
        public static int Tipo_BBDD { get; set; }
        private static ApplicationDbContext _Current_DB_Context;

        public static ApplicationDbContext GetDbContext
        {
            get
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

                switch (Tipo_BBDD)
                {
                    case 1: // SQL SERVER
                        throw new System.Exception("No existe definido para el tipo de base de datos : " + Tipo_BBDD);
                        //optionsBuilder.UseSqlServer(CurrentCnx);
                        break;

                    case 2:
                        optionsBuilder.UseNpgsql(CurrentCnx);
                        break;

                    default:
                        throw new System.Exception("No existe definicio para el tipo de base de datos : " + Tipo_BBDD);
                }

                _Current_DB_Context = new ApplicationDbContext(optionsBuilder.Options);
                return _Current_DB_Context;
            }
        }

        //        private static readonly object _EntityLocker = new object();
        //        private static readonly object _TransactionLocker = new object();
        //        private static ApplicationDbContext _Current_DB_Context;
        //        //private static IDbContextTransaction _Current_Transaction;

        //        private Factory()
        //        {
        //        }

        //        public static string CurrentCnx { get; set; }

        //        public static ApplicationDbContext GetCurrentDbContext
        //        {
        //            get
        //            {
        //#if (TESTING)
        //                if (_ExcpMgr == null) _ExcpMgr = CnxEntity.EntLibExceptionManager.getInstancia();
        //                if (CnxEntity.xCnxMetodo.stringConexion == null) CnxEntity.xCnxMetodo.ArmarCadenaConexion("bd_teCorp");
        //#endif
        //                lock (_EntityLocker)
        //                {
        //                    if (_Current_DB_Context != null) return _Current_DB_Context;
        //                    else
        //                    {
        //                        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //                        optionsBuilder.UseNpgsql(CurrentCnx);
        //                        _Current_DB_Context = new ApplicationDbContext(optionsBuilder.Options);
        //                        return _Current_DB_Context;
        //                    }
        //                }
        //            }
        //        }

        //        public static ApplicationDbContext GetDbContext
        //        {
        //            get
        //            {
        //#if (TESTING)
        //                if (_ExcpMgr == null) _ExcpMgr = CnxEntity.EntLibExceptionManager.getInstancia();
        //                if (CnxEntity.xCnxMetodo.stringConexion == null) CnxEntity.xCnxMetodo.ArmarCadenaConexion("bd_teCorp");
        //#endif
        //                lock (_EntityLocker)
        //                {
        //                 var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //                 optionsBuilder.UseNpgsql(CurrentCnx);
        //                 _Current_DB_Context = new ApplicationDbContext(optionsBuilder.Options);
        //                 return _Current_DB_Context;
        //                 }
        //            }
        //        }

        //        public static IDbContextTransaction GetCurrentTransaction
        //        {
        //            get
        //            {
        //                lock (_TransactionLocker)
        //                {
        //                    if (_Current_Transaction != null) return _Current_Transaction;
        //                    else
        //                    {
        //                        _Current_Transaction = _Current_DB_Context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        //                        return _Current_Transaction;
        //                    }
        //                }
        //            }
        //        }

        //        public static TransactionScope createTransaction()
        //        {
        //                lock (_TransactionLocker)
        //                {
        //                    TransactionScopeOption scopeOption = new TransactionScopeOption();
        //                    scopeOption = TransactionScopeOption.Required;

        //                    TransactionOptions transOptions = new TransactionOptions();
        //                    transOptions.IsolationLevel = System.Transactions.IsolationLevel.Serializable;

        //                    return new TransactionScope(scopeOption, transOptions);
        //                }
        //        }
    }
}