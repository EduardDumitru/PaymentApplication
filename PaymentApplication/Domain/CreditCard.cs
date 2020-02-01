using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApplication.Domain
{
    public class CreditCard
    {
        public string CreditCardNumber { get; set; }
        public string CardHolder { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int? SecurityCode { get; set; }
        public decimal Amount { get; set; }
    }
}
