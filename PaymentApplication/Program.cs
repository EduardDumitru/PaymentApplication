using System;
using System.Threading.Tasks;
using Autofac;
using PaymentApplication.Domain;
using PaymentApplication.Services;

namespace PaymentApplication
{
    public static class Program
    {
        private static async Task Main()
        {
            var creditCard = new CreditCard()
            {
                Amount = 2,
                CardHolder = "FirstName LastName",
                ExpirationDate = new DateTime(2021, 05, 29),
                CreditCardNumber = "5342123452999"
            };

            var container = ContainerConfig.Configure();
            await using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<IExecutePayment>();
                await app.ValidateAndPay(creditCard);
            }

            Console.ReadLine();
        }
    }
}
