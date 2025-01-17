using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace wDualAssistence
{
    public class Program
    {
        public static void Main(string[] args)
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));

            var repo = log4net.LogManager.CreateRepository(
            Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            var logger = LogManager.GetLogger(typeof(Program));
            logger.Info("Application - WebService starting");

#if DEBUG

            CreateDebug_WebHostBuilder(args).Build().Run();
#elif RELEASE
            CreateRelease_HostBuilder(args).Build().Run();
#endif
        }

        public static IWebHostBuilder CreateDebug_WebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddCommandLine(args)
                        .Build();

            return WebHost.CreateDefaultBuilder(args)
                    //.ConfigureLogging((context, logging) => {
                    //    logging.ClearProviders();
                    //    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    //    logging.AddDebug();
                    //    logging.AddConsole(); //EventSource, EventLog, TraceSource, ApplicationInsights
                    //})
                    .UseEnvironment("Development")
                    .UseConfiguration(config)
                    .UseIISIntegration()
                    .UseUrls("http://*:5000")
                    .UseStartup<Startup>();
        }

        public static IWebHostBuilder CreateRelease_HostBuilder(string[] args)
        {

            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddCommandLine(args)
            .Build();

            return WebHost.CreateDefaultBuilder(args)
                    .UseEnvironment("Production")
                    .UseConfiguration(config)
                    .UseIISIntegration()
                    .UseStartup<Startup>();
        }
    }
}
