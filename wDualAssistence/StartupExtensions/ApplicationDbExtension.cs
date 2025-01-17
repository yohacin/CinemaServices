using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace wDualAssistence.StartupExtensions
{
    public static class ApplicationDbExtension
    {
        public static void AddApplicationDbInstance(this IServiceCollection services, int Tipo_BBDD, string CurrentCnx)
        {
            switch (Tipo_BBDD)
            {
                case 1: // SQL SERVER+
                    services.AddDbContext<ApplicationDbContext>(options =>
                                options.UseSqlServer(CurrentCnx));
                    break;

                case 2:
                    services.AddDbContext<ApplicationDbContext>(options =>
                                options.UseNpgsql(CurrentCnx));
                    break;
            }
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }
    }
}