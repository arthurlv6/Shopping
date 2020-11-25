using AutoMapper;
using Inventory;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using System;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Inventory
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var services = builder.Services;
            services.AddAutoMapper(typeof(ModelProfile).GetTypeInfo().Assembly);
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("DefaultConnection")), ServiceLifetime.Scoped);
            services.AddScoped<IStockRepo, StockRepo>();
            services.AddScoped<IStockService, StockService>();
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }
    }
}
