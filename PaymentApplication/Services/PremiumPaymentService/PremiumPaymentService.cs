using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApplication.Services
{
    public class PremiumPaymentService : IPremiumPaymentService
    {
        public async Task<bool> VerifyPayment(decimal amount)
        {
            try
            {
                // We should call the external service for premium payment and check if it is available
                // That's why this is in a try catch
                if (amount > 500)
                {
                    for (var tries = 0; tries < 3; tries++)
                    {
                        var success = await VerifyPayment(amount);
                        if (success)
                        {
                            Console.WriteLine("Premium Payment has been processed successfully…");
                            return true;
                        }

                        Console.WriteLine("There was an error. We will retry the premium payment process.");
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    throw new Exception("You cannot use this service.");
                }

                Console.WriteLine("There was an error. We couldn't finalize the premium payment process");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
