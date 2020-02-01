using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApplication.Shared
{
    public static class SharedValues
    {
        public const decimal AmountLowestLimit = 0M;
        public const decimal AmountCheapLimit = 21M;
        public const decimal AmountExpensiveLimit = 500M;
        public const int CreditCardNumberDigitsNumber = 16;
        public const int SecurityNumberDigitsNumber = 3;
    }
}
