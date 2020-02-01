using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PaymentApplication.Domain;

namespace PaymentApplication.Services
{
    public interface IPaymentManager
    {
        Task<bool> ValidateAndPay(CreditCard creditCard);
        Task SaveToJson(CreditCard creditCard);
    }
}
