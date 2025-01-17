using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPU = Entities.Public;
using BPU = Business.Public;
using wDualAssistence.Controllers;
using wDualAssistence.Utility.Controllers;
using Data;
using Microsoft.EntityFrameworkCore;

namespace wDualAssistence.Hubs
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHubContext<EncoladorHub, IClock> _clockHub;
        protected DbContextOptionsBuilder<ApplicationDbContext> _optionsBuilder;

        public Worker(ILogger<Worker> logger, IHubContext<EncoladorHub, IClock> encoladorHub)
        {
            _logger = logger;
            _clockHub = encoladorHub;
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
            //case 2:
            _optionsBuilder.UseNpgsql(Startup.CurrentCnx);
            //        break;
            //}
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (ApplicationDbContext _appCnx = new ApplicationDbContext(_optionsBuilder.Options))
                {
                    //bool x = new EventoServiceController().VerificarSincronizacion();

                    _logger.LogInformation($"Jobs...: {DateTime.Now}");

                    //new WorkerController().VerificarSincronizacion();
                    //new EngagementController().NotificarAvance();
                    new EngagementManager(_appCnx).NotificarAvance();

                    await Task.Delay(1000 * 60 * 1/*Startup.time*/); // 1 min (millisegundo - segundo - minuto)
                }
            }
        }
    }
}