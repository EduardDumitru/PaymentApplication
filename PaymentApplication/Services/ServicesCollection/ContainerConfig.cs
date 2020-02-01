using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using Autofac;
using Autofac.Core;

namespace PaymentApplication.Services
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ExecutePayment>().As<IExecutePayment>();
            builder.RegisterType<CheapPaymentService>().As<ICheapPaymentService>();
            builder.RegisterType<ExpensivePaymentService>().As<IExpensivePaymentService>();
            builder.RegisterType<PremiumPaymentService>().As<IPremiumPaymentService>();

            return builder.Build();
        }
    }
}
