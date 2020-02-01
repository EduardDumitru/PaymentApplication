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

            if (creditCard.Amount > (decimal) SharedValues.AmountExpensiveLimit)
            {
                return await PremiumPaymentVerification(creditCard);
            }

            return false;
        }

        public async Task SaveToJson(CreditCard creditCard)
        {
            const string filePath = @"d:\payments.json";
            var creditCards = new List<CreditCard>();
            // Find if file exists
            if (File.Exists(filePath))
            {
                // Take all of the existing payments from the json file and add the one that we just made to it
                // This way we won't delete the existing payments from the file
                using var reader = new StreamReader(filePath);
                string json = reader.ReadToEnd();
                creditCards = JsonConvert.DeserializeObject<List<CreditCard>>(json);
            }
            else
            {
                // serialize JSON to a string and then write string to a file
                File.WriteAllText(filePath, JsonConvert.SerializeObject(creditCard));
            }

            // serialize JSON directly to a file
            await using (StreamWriter file = File.CreateText(filePath))
            {
                creditCards.Add(creditCard);
                var serializer = new JsonSerializer();
                serializer.Serialize(file, creditCards);
            }

            Console.WriteLine("It was saved to JSON with success");
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

            if (creditCard.SecurityCode.HasValue && creditCard.SecurityCode != SharedValues.SecurityNumberDigitsNumber)
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

            return await _cheapPaymentService.VerifyPayment(creditCard.Amount);
        }

        private async Task<bool> PremiumPaymentVerification(CreditCard creditCard)
        {
            for (var tries = 0; tries < 3; tries++)
            {
                var success = await _premiumPaymentService.VerifyPayment(creditCard.Amount);
                if (success)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
