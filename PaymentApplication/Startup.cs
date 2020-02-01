using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PaymentApplication.Services;
using PaymentApplication.Services.ServicesCollection;

namespace PaymentApplication
{
    internal class Startup
    {
        private IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationRoot>(Configuration);
            ServicesCollection.CreateDependencies(services);
        }
    }
}
