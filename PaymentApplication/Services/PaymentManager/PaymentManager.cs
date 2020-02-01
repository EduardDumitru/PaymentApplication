using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentApplication.Domain;
using PaymentApplication.Shared;


namespace PaymentApplication.Services
{
    public class PaymentManager : IPaymentManager
    {
        private readonly IPremiumPaymentService _premiumPaymentService;
        private readonly IExpensivePaymentService _expensivePaymentService;
        private readonly ICheapPaymentService _cheapPaymentService;

        public PaymentManager(IPremiumPaymentService premiumPaymentService,
            IExpensivePaymentService expensivePaymentService, ICheapPaymentService cheapPaymentService)
        {
            _premiumPaymentService = premiumPaymentService;
            _expensivePaymentService = expensivePaymentService;
            _cheapPaymentService = cheapPaymentService;
        }

        public async Task<bool> ValidateAndPay(CreditCard creditCard)
        {
            var canContinue = await MandatoryVerification(creditCard);

            if (!canContinue)
            {
                return false;
            }

            if (creditCard.Amount < SharedValues.AmountCheapLimit)
            {
                return await CheapPaymentVerification(creditCard);
            }

            if (creditCard.Amount >= SharedValues.AmountCheapLimit && creditCard.Amount <=
                SharedValues.AmountExpensiveLimit)
            {
                return await ExpensivePaymentVerification(creditCard);
            }

            if (creditCard.Amount > SharedValues.AmountExpensiveLimit)
            {
                return await PremiumPaymentVerification(creditCard);
            }

            return false;
        }

        private static Task<bool> MandatoryVerification(CreditCard creditCard)
        {
            var success = true;
            if (string.IsNullOrWhiteSpace(creditCard.CardHolder))
            {
                Console.WriteLine("Card Holder cannot be null or empty");
                success = false;
            }

            if (string.IsNullOrWhiteSpace(creditCard.CreditCardNumber))
            {
                Console.WriteLine("Credit Card Number cannot be null or empty");
                success = false;
            }

            if (!creditCard.CreditCardNumber.All(x => x >= '0' && x <= '9'))
            {
                Console.WriteLine("Credit Card Number must contain only digits");
                success = false;
            }

            if (creditCard.CreditCardNumber.Length != SharedValues.CreditCardNumberDigitsNumber)
            {
                Console.WriteLine("Credit Card Number must be exactly " +
                                  $"{SharedValues.CreditCardNumberDigitsNumber} digits");
                success = false;
            }

            if (creditCard.ExpirationDate.Date <= DateTime.Now.Date)
            {
                Console.WriteLine("Expiration Date cannot be in the past");
                success = false;
            }

            if (creditCard.SecurityCode.HasValue 
                && creditCard.SecurityCode.Value.ToString().Length != SharedValues.SecurityNumberDigitsNumber)
            {
                Console.WriteLine("Security Code must be exactly " +
                                  $"{SharedValues.SecurityNumberDigitsNumber} digits");
                success = false;
            }

            if (creditCard.Amount <= SharedValues.AmountLowestLimit)
            {
                Console.WriteLine($"Amount must be higher than {SharedValues.AmountLowestLimit}");
                success = false;
            }

            return Task.FromResult(success);
        }

        //I made separate methods because maybe someone will want to add some more logic to them outside their class
        private async Task<bool> CheapPaymentVerification(CreditCard creditCard)
        {
            return await _cheapPaymentService.VerifyPayment(creditCard.Amount);
        }

        private async Task<bool> ExpensivePaymentVerification(CreditCard creditCard)
        {
            var success = await _expensivePaymentService.VerifyPayment(creditCard.Amount);
            if (success)
            {
                return true;
            }

            Console.WriteLine("Expensive Payment could not be processed. We will try to process it with Cheap Payment.");
            return await _cheapPaymentService.VerifyPayment(creditCard.Amount);
        }

        private async Task<bool> PremiumPaymentVerification(CreditCard creditCard)
        {
            return await _premiumPaymentService.VerifyPayment(creditCard.Amount);
        }
    }
}
