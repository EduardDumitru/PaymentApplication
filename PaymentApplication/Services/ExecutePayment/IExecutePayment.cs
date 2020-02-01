using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PaymentApplication.Domain;

namespace PaymentApplication.Services
{
    public interface IExecutePayment
    {
        Task<bool> ValidateAndPay(CreditCard creditCard);
    }
}
