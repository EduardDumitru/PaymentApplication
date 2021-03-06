﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentApplication.Services
{
    public interface IExpensivePaymentService
    {
        public Task<bool> VerifyPayment(decimal amount);
    }
}
