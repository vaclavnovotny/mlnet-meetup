using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;
using Microsoft.Extensions.ML;
using MLNET.RealWorld.Controllers;
using MLNET.RealWorld.Db;
using MLNET.RealWorld.Models;
using MLNET.RealWorld.Services;

namespace MLNET.RealWorld
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MLContext>();
            services.AddTransient<DataLoader>();
            services.AddTransient<DbModelManager>();
            services.AddTransient<SpamDetector>();
            services.AddTransient<SpamDetectorTrainer>();

            services.AddDbContext<SpamDetectorDbContext>(builder => builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            Uri.TryCreate($"http://127.0.0.1:5000/{nameof(ClassifierController).Replace("Controller", string.Empty)}/{nameof(ClassifierController.GetClassifierAsZip)}", UriKind.Absolute, out var uri);
            services.AddPredictionEnginePool<SpamInput, SpamPrediction>().FromUri("SpamDetector", uri, TimeSpan.FromMinutes(1));

            services.AddTransient<IStartupFilter, MigrationFilter>();
            services.AddControllers();
            services.AddOpenApiDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
