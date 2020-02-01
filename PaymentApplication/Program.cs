using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PaymentApplication.Domain;
using PaymentApplication.Services;

namespace PaymentApplication
{
    public static class Program
    {
        private static async Task Main()
        {
            var goodCreditCard = new CreditCard
            {
                Amount = 22M,
                CardHolder = "FirstName LastName",
                ExpirationDate = new DateTime(2022, 05, 29),
                CreditCardNumber = "5342123452999213"
            };

            IServiceCollection services = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Get Service and call method
            var paymentManager = serviceProvider.GetService<IPaymentManager>();

            var canSaveToJson = await paymentManager.ValidateAndPay(goodCreditCard);

            if (canSaveToJson)
            {
                await paymentManager.SaveToJson(goodCreditCard);
            }

            // Error scenario
            var errorCreditCard = new CreditCard
            {
                Amount = 2234M,
                CardHolder = "FirstName LastName",
                ExpirationDate = new DateTime(2011, 05, 29),
                CreditCardNumber = "534212345299921322"
            };
            await paymentManager.ValidateAndPay(errorCreditCard);

            Console.ReadLine();
        }
    }
}
