using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentApplication.Services
{
    public class PremiumPaymentService : IPremiumPaymentService
    {
        public Task<bool> VerifyPayment(decimal amount)
        {
            try
            {
                // We should call the external service for premium payment and check if it is available
                // That's why this is in a try catch
                if (amount > 500)
                {
                    Console.WriteLine("Premium Payment has been processed successfully…");
                }
                else
                {
                    throw new Exception("You cannot use this service.");
                }

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.FromResult(false);
            }
        }
    }
}
