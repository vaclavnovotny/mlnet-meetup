using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using MLNET.SpamDetector.RealWorld.Controllers;
using MLNET.SpamDetector.RealWorld.Db;
using MLNET.SpamDetector.RealWorld.Models;
using MLNET.SpamDetector.RealWorld.Services;

namespace MLNET.SpamDetector.RealWorld
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
            services.AddTransient<Services.SpamDetector>();
            services.AddTransient<SpamDetectorTrainer>();

            services.AddDbContext<SpamDetectorDbContext>(builder => builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            Uri.TryCreate($"http://127.0.0.1:5000/{nameof(ClassifierController).Replace("Controller", string.Empty)}/{nameof(ClassifierController.GetClassifierAsZip)}", UriKind.Absolute, out var uri);
            services.AddPredictionEnginePool<SpamInput, SpamPrediction>().FromUri(ModelsCatalog.SpamDetector, uri, TimeSpan.FromMinutes(5));

            services.AddTransient<IStartupFilter, MigrationFilter>();
            services.AddControllers();
            services.AddOpenApiDocument(settings => {
                settings.Title = "Spam Detector";
            });
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
