using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MLNET.RealWorld.Db;

namespace MLNET.RealWorld {
    public class MigrationFilter : IStartupFilter
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MigrationFilter(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) {
            using (var realWorldDbContext = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<SpamDetectorDbContext>())
            {
                realWorldDbContext.Database.Migrate();
            }
            return next;
        }
    }
}