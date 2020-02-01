using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentApplication.Services.ServicesCollection
{
    public static class ServicesCollection
    {
        public static void CreateDependencies(IServiceCollection services)
        {
            //I made them singleton because I'm only applying logic
            services.AddSingleton<IPaymentManager, PaymentManager>();
            services.AddSingleton<ICheapPaymentService, CheapPaymentService>();
            services.AddSingleton<IExpensivePaymentService, ExpensivePaymentService>();
            services.AddSingleton<IPremiumPaymentService, PremiumPaymentService>();
            services.AddScoped<IFileService, FileService>();
        }
    }
}
