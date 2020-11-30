using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Payment;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Payment
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var services = builder.Services;
            services.AddScoped<IStockRepo, StockRepo>();
        }
    }
}
