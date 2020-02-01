using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PaymentApplication.Domain;


namespace PaymentApplication.Services
{
    public class ExecutePayment : IExecutePayment
    {
        private readonly IPremiumPaymentService _premiumPaymentService;
        private readonly IExpensivePaymentService _expensivePaymentService;
        private readonly ICheapPaymentService _cheapPaymentService;

        public ExecutePayment(IPremiumPaymentService premiumPaymentService,
            IExpensivePaymentService expensivePaymentService, ICheapPaymentService cheapPaymentService)
        {
            _premiumPaymentService = premiumPaymentService;
            _expensivePaymentService = expensivePaymentService;
            _cheapPaymentService = cheapPaymentService;
        }

        public async Task<bool> ValidateAndPay(CreditCard creditCard)
        {
            if (creditCard.Amount < (decimal) AmountLimits.CheapLimit)
            {
                return await CheapPaymentVerification(creditCard);
            }

            if (creditCard.Amount >= (decimal) AmountLimits.CheapLimit && creditCard.Amount <=
                (decimal) AmountLimits.ExpensiveLimit)
            {
                return await ExpensivePaymentVerification(creditCard);
            }

            if (creditCard.Amount > (decimal) AmountLimits.ExpensiveLimit)
            {
                return await PremiumPaymentVerification(creditCard);
            }

            return false;
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
