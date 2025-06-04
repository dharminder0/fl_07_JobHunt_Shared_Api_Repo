using Autofac;
using Microsoft.OpenApi.Models;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json;
using System.Reflection;
using VendersCloud.Business;
using VendersCloud.Common.Configuration;
using VendersCloud.Common.Settings;
using VendersCloud.Common.Utils;
using ConfigurationManager = VendersCloud.Common.Configuration.ConfigurationManager;

namespace VendersCloud.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration= configuration;
        }
        public IConfiguration Configuration { get; }
        public ServiceProvider serviceProvider { get; set; }

        //Tis method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            InitSettings();
            var conn = Configuration.GetConnectionString("");
            services.AddScoped<RequireAuthorizationFilter>();
            //// Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Venders Cloud Service", Version = "v1" });
                // Authorization header
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Authorization header using the Bearer scheme. <br/>
                      Enter 'Bearer' [space] and then your token in the text input below.
                      <br/> Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityDefinition("token", new OpenApiSecurityScheme
                {
                    Description = @"JWT user encrypted token header",
                    Name = "token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityDefinition("lang", new OpenApiSecurityScheme
                {
                    Description = @"Language code for request",
                    Name = "lang",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference
                      {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                      },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,

                    },
                    new List<string>()
                  }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference
                      {
                        Type = ReferenceType.SecurityScheme,
                        Id = "token"
                      },
                      Scheme = "oauth2",
                      Name = "token",
                      In = ParameterLocation.Header,

                    },
                    new List<string>()
                  }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference
                      {
                        Type = ReferenceType.SecurityScheme,
                        Id = "lang"
                      },
                      Scheme = "oauth2",
                      Name = "lang",
                      In = ParameterLocation.Header,

                    },
                    new List<string>()
                  }
                });
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
                //c.SchemaFilter<SwaggerExcludeFilter>();
                //c.OperationFilter<OpenApiParameterIgnoreFilter>();
            });
            var assembliesToScan = new[] {
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(VendersCloud.Business.IDependency)),
                Assembly.GetAssembly(typeof(VendersCloud.Data.IDependency)),
                 Assembly.GetAssembly(typeof(VendersCloud.Common.IDependency)),
                  Assembly.GetAssembly(typeof(VendersCloud.WebApi.IDependency)),
            };
         
            //  services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan).AsPublicImplementedInterfaces();

            services.AddHttpContextAccessor();
            var ioc = new IoC(() => {
                var builder = new ContainerBuilder();
                builder.RegisterAssemblyTypes(assembliesToScan)
                    .Where(c => c.Name.EndsWith("Service") ||
                                c.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces();
                return builder.Build();
            });
            // register services only
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
                .Where(c => c.Name.EndsWith("Service") || c.Name.EndsWith("Repository")).AsPublicImplementedInterfaces();
            //  services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan).AsPublicImplementedInterfaces();

            services.AddScoped<IpHelper>();

            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            //// for handling error The collection type 'Newtonsoft.Json.Linq.JObject or JToken or JArray' is not supported
            //// requires https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.NewtonsoftJson/


            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            //add CORS for origins to be allowed

            services.AddCors(options => {

                options.AddPolicy("AllowedOrigins",
                        builder => {
                            builder.AllowAnyMethod().AllowAnyHeader();
                            //if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["AllowedOrigins"]) && ConfigurationManager.AppSettings["AllowedOrigins"] != "*")
                            if (!string.IsNullOrWhiteSpace(GlobalSettings.AllowedOrigins) && GlobalSettings.AllowedOrigins != "*")
                            {
                                builder.WithOrigins(GlobalSettings.AllowedOrigins.Split(','));
                            }
                            else
                            {
                                builder.AllowAnyOrigin();
                            }
                        });
            });


            services.AddSignalR();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // we are using cloudflare and that prevents it
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CCS API V1");
            });
        }

        private void InitSettings()
        {
            const string CONNECTIONS_SECTION = "ConnectionStrings";
            const string APPSETTINGS_SECTION = "AppSettings";
            //Connections
            if (Configuration.GetSection(CONNECTIONS_SECTION).Exists())
            {
                foreach (var item in Configuration.GetSection(CONNECTIONS_SECTION).AsEnumerable())
                {
                    var key = item.Key.Replace(CONNECTIONS_SECTION, "");
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        ConfigurationManager.ConnectionStrings.Add(key.TrimStart(':'), new ConfigConnection { ConnectionString = item.Value });
                    }
                }
            }

            //AppSettings
            if (Configuration.GetSection(APPSETTINGS_SECTION).Exists())
            {
                foreach (var item in Configuration.GetSection(APPSETTINGS_SECTION).AsEnumerable())
                {
                    var key = item.Key.Replace(APPSETTINGS_SECTION, "");
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        ConfigurationManager.AppSettings.Add(key.TrimStart(':'), item.Value);
                    }
                }
            }
        }
    }
}
