using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        //public static readonly LoggerFactory LoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });
        //public static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        //{
        //    builder.AddConsole(); // Agrega el proveedor de logging de consola
        //});

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // if (!optionsBuilder.IsConfigured)
            //     optionsBuilder.UseNpgsql("Server=172.16.10.42\\Desarrollo2014;User ID=sa;Password=Desarrollo2014.;Database=bd_Jugos");
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseLoggerFactory(LoggerFactory); // Usa la configuración de logging en DbContext
            //}
#if (DEBUG2)
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(LoggerFactory);
#endif
        }

        //public IEnumerable<dynamic> CollectionFromSql(string Sql, NpgsqlParameter[] Parameters)
        //{
        //    using (var cmd = this.Database.GetDbConnection().CreateCommand())
        //    {
        //        cmd.CommandText = Sql;
        //        if (cmd.Connection.State != ConnectionState.Open)
        //            cmd.Connection.Open();
        //        cmd.Parameters.AddRange(Parameters);

        //        //foreach (param in Parameters)
        //        //{
        //        //    DbParameter dbParameter = cmd.CreateParameter();
        //        //    dbParameter.ParameterName = param.Key;
        //        //    dbParameter.Value = param.Value;
        //        //    cmd.Parameters.Add(dbParameter);
        //        //}

        //        //var retObject = new List<dynamic>();
        //        using (var dataReader = cmd.ExecuteReader())
        //        {
        //            while (dataReader.Read())
        //            {
        //                var dataRow = GetDataRow(dataReader);
        //                yield return dataRow;
        //            }

        //            if (!dataReader.HasRows)
        //            {
        //                var dataRow2 = new ExpandoObject() as IDictionary<string, object>;
        //                for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
        //                    dataRow2.Add(dataReader.GetName(fieldCount), null);
        //                yield return dataRow2;
        //            }
        //        }
        //    }
        //}

        private dynamic GetDataRow(DbDataReader dataReader)
        {
            var dataRow = new ExpandoObject() as IDictionary<string, object>;
            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
            return dataRow;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //quuitando Campos para no requeridos en BASE
            builder.Entity<Entities.Security.Usuario>().Ignore(table => table.perfil);
            builder.Entity<Entities.Public.Tarea>().Ignore(table => table.categoria);
            builder.Entity<Entities.Public.Empleado>().Ignore(table => table.listaTurno);
            builder.Entity<Entities.Public.Empleado>().Ignore(table => table.jornada);
            builder.Entity<Entities.Public.Empleado>().Ignore(table => table.sucursal);
            builder.Entity<Entities.Public.Dia_Feriado>().Ignore(table => table.ciudad);
            builder.Entity<Entities.Public.Marcacion_Biometrico>().Ignore(table => table.usuario);
            builder.Entity<Entities.Public.Marcacion_Biometrico>().Ignore(table => table.empresa);
            builder.Entity<Entities.Public.Marcacion_Biometrico>().Ignore(table => table.formato);
            builder.Entity<Entities.Public.Sucursal>().Ignore(table => table.ciudad);

            builder.Entity<Entities.Notificador.Bitacora_Sincronizar>().Ignore(table => table.lstBitacora);
            builder.Entity<Entities.Notificador.Bitacora_Sincronizar>().Ignore(table => table.fecha_fin);
            builder.Entity<Entities.Notificador.Bitacora_Sincronizar>().Ignore(table => table.cant_metodos);
            builder.Entity<Entities.Notificador.Campana_Detalle>().HasKey(table => new { table.codigo_asignado, table.codigo_campana, table.codigo_grupo });
            builder.Entity<Entities.Notificador.Contacto>().HasKey(table => new { table.codigo_grupo, table.codigo_contacto });

            ///llaves copuestas
            //builder.Entity<Entities.Security.PerfilEmpresa>().HasKey(table => new {table.codigo_perfil,table.codigo_empresa});
        }

        private IDbContextTransaction _currentTransaction;

        public IEnumerable<dynamic> CollectionFromSql(string Sql)
        {
            using (var cmd = this.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = Sql;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                //cmd.Parameters.AddRange(Parameters);

                //foreach (param in Parameters)
                //{
                //    DbParameter dbParameter = cmd.CreateParameter();
                //    dbParameter.ParameterName = param.Key;
                //    dbParameter.Value = param.Value;
                //    cmd.Parameters.Add(dbParameter);
                //}

                //var retObject = new List<dynamic>();
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var dataRow = GetDataRow(dataReader);
                        yield return dataRow;
                    }

                    if (!dataReader.HasRows)
                    {
                        var dataRow2 = new ExpandoObject() as IDictionary<string, object>;
                        for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                            dataRow2.Add(dataReader.GetName(fieldCount), null);
                        yield return dataRow2;
                    }
                }
            }
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _currentTransaction?.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public DbContext dbContext => this;

        public bool HasActiveTransaction => _currentTransaction != null;

        //------------------------------- Usuario  y Perfiles
        public DbSet<Entities.Security.Usuario> Usuario { get; set; }

        public DbSet<Entities.Security.Usuario_Acceso> Usuario_Acceso { get; set; }
        public DbSet<Entities.Security.Modulo> Modulo { get; set; }
        public DbSet<Entities.Security.Acceso> Acceso { get; set; }
        public DbSet<Entities.Security.Perfil> Perfil { get; set; }
        public DbSet<Entities.Security.Perfil_Acceso> Perfil_Acceso { get; set; }
        public DbSet<Entities.Security.LogSync> LogSync { get; set; }
        public DbSet<Entities.Security.Clasificador> ClasificadorSecuriry { get; set; }

        //------------------------------- helpers
        public DbSet<Entities.Public.Empresa> Empresa { get; set; }

        public DbSet<Entities.Public.Empleado> Empleado { get; set; }
        public DbSet<Entities.Public.Turno> Turno { get; set; }
        public DbSet<Entities.Public.Marcacion> Marcacion { get; set; }
        public DbSet<Entities.Public.AlertaMarcacionErronea> AlertaMarcacionErronea { get; set; }
        public DbSet<Entities.Public.Foto_Marcacion> Foto_Marcacion { get; set; }
        public DbSet<Entities.Public.Empleado_Empresa> Empleado_Empresa { get; set; }
        public DbSet<Entities.Public.Permanencia> Permanencia { get; set; }
        public DbSet<Entities.Public.Categoria_Tarea> Categoria_Tarea { get; set; }
        public DbSet<Entities.Public.Tarea> Tarea { get; set; }
        public DbSet<Entities.Public.Engagement> Engagement { get; set; }
        public DbSet<Entities.Public.Detalle_Hora_Engagement> Detalle_Hora_Engagement { get; set; }
        public DbSet<Entities.Public.Alerta_Engagement> Alerta_Engagement { get; set; }
        public DbSet<Entities.Public.Responsable_Engagement> Responsable_Engagement { get; set; }
        public DbSet<Entities.Public.Tarea_Engagement> Tarea_Engagement { get; set; }
        public DbSet<Entities.Public.Engagement_Empleado> Engagement_Empleado { get; set; }
        public DbSet<Entities.Public.Detalle_Engagement_Empleado> Detalle_Engagement_Empleado { get; set; }
        public DbSet<Entities.Public.Empleado_Hoja_Trabajo> Empleado_Hoja_Trabajo { get; set; }
        public DbSet<Entities.Public.Dia_Feriado> Dia_Feriado { get; set; }
        public DbSet<Entities.Public.Cargo_Engagement> Cargo_Engagement { get; set; }
        public DbSet<Entities.Public.RespuestaWS> RespuestaWS { get; set; }
        public DbSet<Entities.Public.ViewLogin> ViewLogin { get; set; }
        public DbSet<Entities.Public.Excepcion> Excepcion { get; set; }
        public DbSet<Entities.Public.ConfigParametro> ConfigParametro { get; set; }
        public DbSet<Entities.Public.MarcacionErronea> MarcacionErronea { get; set; }
        public DbSet<Entities.Public.Engagement_Modificacion_Solicitud> Engagement_Modificacion_Solicitud { get; set; }
        public DbSet<Entities.Public.Area_Engagement> Area_Engagement { get; set; }
        public DbSet<Entities.Public.Encuesta_Config> Encuesta_Config { get; set; }
        public DbSet<Entities.Public.Encuesta> Encuesta { get; set; }
        public DbSet<Entities.Public.Foto_Empresa> Foto_Empresa { get; set; }
        public DbSet<Entities.Public.Foto_Empleado> Foto_Empleado { get; set; }
        public DbSet<Entities.Public.Jornada> Jornada { get; set; }
        public DbSet<Entities.Public.Detalle_Jornada> Detalle_Jornada { get; set; }
        public DbSet<Entities.Public.Formato_Marcacion_Biometrico> Formato_Marcacion_Biometrico { get; set; }
        public DbSet<Entities.Public.Pais> Pais { get; set; }
        public DbSet<Entities.Public.Ciudad> Ciudad { get; set; }
        public DbSet<Entities.Public.Sucursal> Sucursal { get; set; }
        public DbSet<Entities.Public.Marcacion_Biometrico> Marcacion_Biometrico { get; set; }
        public DbSet<Entities.Public.Detalle_Marcacion_Biometrico> Detalle_Marcacion_Biometrico { get; set; }
        public DbSet<Entities.Public.Turno_Plantilla> Turno_Plantilla { get; set; }


        //------------------------------- Notificador
        public DbSet<Entities.Notificador.Credencial_Correo> Credencial_Correo { get; set; }

        public DbSet<Entities.Notificador.Campana> Campana { get; set; }
        public DbSet<Entities.Notificador.Grupo> Grupo { get; set; }
        public DbSet<Entities.Notificador.Contacto> Contacto { get; set; }
        public DbSet<Entities.Notificador.Campana_Detalle> Campana_Detalle { get; set; }
        public DbSet<Entities.Notificador.Mensaje> Mensaje { get; set; }
        public DbSet<Entities.Notificador.Notificacion> Notificacion { get; set; }
        public DbSet<Entities.Notificador.Plantilla> Plantilla { get; set; }
        public DbSet<Entities.Notificador.Tipo_Clasificador> TipoClasificador { get; set; }
        public DbSet<Entities.Notificador.Clasificador> Clasificador { get; set; }
        public DbSet<Entities.Notificador.Campana_Plantilla> Campana_Plantilla { get; set; }
        public DbSet<Entities.Notificador.Query> Query { get; set; }
        public DbSet<Entities.Notificador.Adjunto> Adjunto { get; set; }
        public DbSet<Entities.Notificador.Query_Campana> Query_Campana { get; set; }
        public DbSet<Entities.Notificador.Config_Sincronizar> Config_Sincronizar { get; set; }
        public DbSet<Entities.Notificador.Bitacora_Sincronizar> Bitacora_Sincronizar { get; set; }




        //------------------------------- Config
        public DbSet<Entities.Config.Tipo_Archivo> Tipo_Archivo { get; set; }

        //------------------------------- helpers
        public DbSet<Entities.Helpers.TreeCollection> TreeCollection { get; set; }

        public DbSet<Entities.Helpers.MarcacionHelpers> MarcacionHelpers { get; set; }
        public DbSet<Entities.Helpers.PermanenciaHelper> PermanenciaHelper { get; set; }
        public DbSet<Entities.Helpers.MarcacionErroneaHelper> MarcacionErroneaHelper { get; set; }
        public DbSet<Entities.Helpers.AsignacionHelper> AsignacionHelper { get; set; }
        public DbSet<Entities.Helpers.SeguimientoEngagementHelper> SeguimientoEngagementHelper { get; set; }
        public DbSet<Entities.Helpers.ViewDashboardReport> ViewDashboardReport { get; set; }
        public DbSet<Entities.Helpers.ViewDashboardMarcacionEmpleadoReport> ViewDashboardMarcacionEmpleadoReport { get; set; }
        public DbSet<Entities.Helpers.AsignacionEmpleadoHelper> AsignacionEmpleadoHelper { get; set; }
        public DbSet<Entities.Helpers.AvanceEngagementHelper> AvanceEngagementHelper { get; set; }
        public DbSet<Entities.Helpers.NotificarAlertaHelper> NotificarAlertaHelper { get; set; }
        public DbSet<Entities.Helpers.PlaneacionEngagementHelper> PlaneacionEngagementHelper { get; set; }
    }
}