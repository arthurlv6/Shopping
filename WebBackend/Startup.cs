using AutoMapper.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using WebBackend;

[assembly: FunctionsStartup(typeof(Startup))]
namespace WebBackend
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var temp = Environment.GetEnvironmentVariable("InventoryApi");
            builder.Services.AddHttpClient<InventoryService>(
                client => client.BaseAddress = new Uri(temp));
        }
    }
}
