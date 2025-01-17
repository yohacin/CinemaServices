using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wDualAssistence.Controllers;
using wDualAssistence.Utility.Controllers;

namespace wDualAssistence.Hubs
{
    public interface ITurnoWorkService
    {
        void DoSomeWork();
    }

    public class TurnoWorkService : ITurnoWorkService
    {
        protected DbContextOptionsBuilder<ApplicationDbContext> _optionsBuilder;

        public TurnoWorkService()
        {
            InstanciarOptionsBuilder();
        }

        private void InstanciarOptionsBuilder()
        {
            _optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            //switch (Startup.Tipo_BBDD)
            //{
            //    case 1: // SQL SERVER+
            //        _optionsBuilder.UseSqlServer(Startup.CurrentCnx);
            //        break;
            //    case 2:
            _optionsBuilder.UseNpgsql(Startup.CurrentCnx);
            //        break;
            //}
        }

        public void DoSomeWork()
        {
            using (ApplicationDbContext _appCnx = new ApplicationDbContext(_optionsBuilder.Options))
            {
                //Console.WriteLine(string.Format("Jobs Turno: - {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                //new WorkerController().VerificarSincronizacion();
                new WorkerManager(_appCnx).VerificarSincronizacion();
            }
        }
    }
}