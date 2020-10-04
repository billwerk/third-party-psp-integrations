using Business.Interfaces;
using Business.PayOne.Factories;
using Business.PayOne.Services;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Persistence.Interfaces;
using Persistence.Mongo;
using Persistence.Services;
using Web.Configuration;
using Web.Handlers;
using Web.Middlewares;

namespace Web {
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            
            services.AddHttpContextAccessor();

            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            
            services.Configure<ProjectSettings>(Configuration.GetSection("ProjectSettings"));
            
            // configure DI for application services
            services.AddSingleton<IGlobalSettings, GlobalSettings>();
            services.AddSingleton(MongoClientFactory.Create(services.BuildServiceProvider().GetService<IGlobalSettings>().MongoHost));
            services.AddScoped<IMongoContext, MongoContext>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IExternalIntegrationInfoFactory, PayOneExternalIntegrationInfoFactory>();
            services.AddScoped<IExternalSettingsValidator, PayOneExternalSettingsValidator>();
            
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(
                    services.BuildServiceProvider().GetService<IMongoClient>() as MongoClient,
                    MongoContext.MongoDatabaseMain, 
                    new MongoStorageOptions
                    {
                        MigrationOptions = new MongoMigrationOptions
                        {
                            MigrationStrategy = new MigrateMongoMigrationStrategy(),
                            BackupStrategy = new CollectionMongoBackupStrategy()
                        },
                        Prefix = "hangfire",
                        CheckConnection = true
                    }));

            // Add the processing server as IHostedService
            services.AddHangfireServer(serverOptions => { serverOptions.ServerName = "Hangfire server 1"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = new BackgroundJobServerOptions
            {
                Queues = new[]
                {
                    "default",
                    "notDefault"
                }
            };

            app.UseHangfireServer(options);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
                app.UseHsts();
            }

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseMiddleware<LogContextValuesMiddleware>();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Get}/{id?}");
            });

            app.UseHangfireDashboard();
        }
    }
}