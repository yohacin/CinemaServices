using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using wDualAssistence.Hubs;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using wDualAssistence.StartupExtensions;
using wDualAssistence.Configs;

namespace wDualAssistence
{
    public class Startup
    {
        public static string appPath = "";
        public static string appUrl = "";
        public static string url_java = "";
        public static string llaveCryptography = "";
        public static int time = 1;
        public static int id_plantilla_alerta = 1;
        public static int id_perfil_defecto = 25;
        public static bool validar_tareas = true;
        public static bool no_validar_marcacion_entrada = false;
        public static string CurrentCnx;
        
        public static ReconocimientoFacialConfig reconocimientoFacialConfig;
        public static MarcacionDesdeBiometricoConfig marcacionDesdeBiometricoConfig;
        public static bool modo_independiente = false;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            appPath = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            CurrentCnx = configuration.GetConnectionString("Connection");
            appUrl = configuration.GetValue<String>("InfoProgram:url");
            url_java = configuration.GetValue<String>("InfoProgram:url_java");
            llaveCryptography = configuration.GetValue<String>("InfoProgram:llave");
            time = (int)configuration.GetValue<Int32>("InfoProgram:time");
            id_plantilla_alerta = (int)configuration.GetValue<Int32>("InfoProgram:id_plantilla_alerta");
            id_perfil_defecto = (int)configuration.GetValue<Int32>("InfoProgram:id_perfil_defecto");
            validar_tareas = (bool)configuration.GetValue<Boolean>("InfoProgram:validar_tareas");
            no_validar_marcacion_entrada = (bool)configuration.GetValue<Boolean>("InfoProgram:no_validar_marcacion_entrada");
            
            reconocimientoFacialConfig = configuration.GetSection("ReconocimientoFacialConfig").Get<ReconocimientoFacialConfig>();
            marcacionDesdeBiometricoConfig = configuration.GetSection("MarcacionDesdeBiometricoConfig").Get<MarcacionDesdeBiometricoConfig>();
            modo_independiente = (bool)configuration.GetValue<Boolean>("ModoIndependiente", false);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Formato Fecha JSON

            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Decimal;
                options.SerializerSettings.DateFormatString = "dd/MM/yyyy";
                //options.SerializerSettings.DateFormatString = "dd-MM-yyyy";
            })
             .AddOData(opt => opt.AddRouteComponents("oData", GetEdmModelForOdata()).Select().Expand().Filter().OrderBy().SetMaxTop(100).Count());

            #endregion Formato Fecha JSON

            services.AddApplicationDbInstance(2, CurrentCnx);
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                        //.AllowCredentials();
                    });
            });

            #region Authentication

            //var key = Encoding.ASCII.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    //.AddJwtBearer(options =>
                    //{
                    //    options.RequireHttpsMetadata = false;
                    //    options.SaveToken = true;
                    //    options.TokenValidationParameters = new TokenValidationParameters
                    //    {
                    //        ValidateIssuerSigningKey = true,
                    //        IssuerSigningKey = new SymmetricSecurityKey(key),
                    //        ValidateIssuer = false,
                    //        ValidateAudience = false
                    //    };
                    //})
                    .AddCookie(options =>
                    {
                        options.LoginPath = new PathString("/Home/Index/");
                        options.AccessDeniedPath = new PathString("/Home/Index/");
                        //options.SlidingExpiration
                        //options.ExpireTimeSpan = new System.TimeSpan(0, 2, 0);
                    });

            #endregion Authentication

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(30);
                hubOptions.HandshakeTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddHostedService<Worker>();

            //services.AddLocalization();
            //services.AddOData();
            services.AddMvc(o => o.InputFormatters.Insert(0, new Helpers.RawRequestBodyFormatter()));
            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            #region "ADICIONAR SWAGGER TO DEBBUGER"

            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new Info { Title = "NetCore API", Version = "v1" });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API - V1", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "My API - V2", Version = "v2" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Ingrese el token de la siguiente manera: 'Bearer {Token}'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            #endregion "ADICIONAR SWAGGER TO DEBBUGER"

            #region Hangfire

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage
                (Configuration.GetConnectionString("Connection"), new PostgreSqlStorageOptions
                {
                    //CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    //SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    //QueuePollInterval = TimeSpan.Zero,
                    //UseRecommendedIsolationLevel = true,
                    //DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
            services.AddTransient<ITurnoWorkService, TurnoWorkService>();

            #endregion Hangfire

            services.AddMvc(options => options.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            #region Register Syncfusion license

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("ORg4AjUWIQA/Gnt2VlhiQlVPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSH9RfkRnW3tbdHBSQ2g=;Mgo+DSMBMAY9C3t2VlhiQlVPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSH9RfkRnW3tbdHJTQWc=;Mjk1MzM0OEAzMjMzMmUzMDJlMzBHdzltUkRiOEVGY29GeDBicUxtYzRpQVNDYVl1blZLeURFcnlQWVZRRFF3PQ==");

            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTc2ODI0QDMxMzcyZTMzMmUzMGVmVUpIUGw4R0VOL1BMYUxlZmpSTS9NUEVGdCtjdEthTytmbStZR2tSbTg9");
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("ODU1MjJAMzEzNzJlMzEyZTMwbEpXem5VWFJKckovT0ttMkZQYm5Fa1R4QncyUlhUUWNyS0g5OEN4U1dtOD0=");

            #endregion Register Syncfusion license

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseODataRouteDebug();
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
            app.UseRouting();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseAuthorization();

            //app.UseHangfireDashboard();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            RecurringJob.AddOrUpdate<ITurnoWorkService>(x => x.DoSomeWork(), "0 5 * * mon-sat"/*Cron.Minutely*/, TimeZoneInfo.Local);

            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCore API V1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "NetCore API V2");
            }
            );

            var BolivianCulture = new CultureInfo("es-BO");
            BolivianCulture.NumberFormat.NumberDecimalSeparator = ".";
            BolivianCulture.NumberFormat.NumberGroupSeparator = ",";

            var NewBolivianCulture = new RequestCulture(BolivianCulture);

            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                BolivianCulture
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = NewBolivianCulture,
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // Localized UI strings.
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();

            app.UseEndpoints(routeBuilder =>
            {
                //routeBuilder.MapControllers();
                routeBuilder.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                ////routeBuilder.EnableDependencyInjection();

                routeBuilder.MapControllerRoute(
                      name: "defaultOdata",
                      pattern: "{controller}/{action=Get}");
                //routeBuilder.MapODataRoute("defaultOdata2", "oData", GetEdmModelForOdata());
                //routeBuilder.Select().Expand().Filter().OrderBy().MaxTop(100).Count();
            });
        }

        //agregar
        public static IEdmModel GetEdmModelForOdata()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Entities.Security.Usuario>("Usuario");
            builder.EntitySet<Entities.Security.Perfil>("Perfil");
            builder.EntitySet<Entities.Public.Empleado>("Empleado");
            builder.EntitySet<Entities.Public.Empresa>("Empresa");
            builder.EntitySet<Entities.Public.Categoria_Tarea>("CategoriaTarea");
            builder.EntitySet<Entities.Public.Tarea>("Tarea");
            builder.EntitySet<Entities.Public.Dia_Feriado>("DiaFeriado");
            builder.EntitySet<Entities.Notificador.Grupo>("Grupo");
            builder.EntitySet<Entities.Public.Area_Engagement>("Area");
            builder.EntitySet<Entities.Public.Cargo_Engagement>("Cargo");
            builder.EntitySet<Entities.Public.Jornada>("Jornada");
            builder.EntitySet<Entities.Public.Formato_Marcacion_Biometrico>("FormatoMarcacionBiometrico");
            builder.EntitySet<Entities.Public.Marcacion_Biometrico>("MarcacionBiometrico");
            builder.EntitySet<Entities.Public.Sucursal>("Sucursal");
            return builder.GetEdmModel();
        }
    }

    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}