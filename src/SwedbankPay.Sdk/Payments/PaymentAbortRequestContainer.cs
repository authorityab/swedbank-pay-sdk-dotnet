﻿namespace SwedbankPay.Sdk.Payments
{
    public class PaymentAbortRequestContainer
    {
        public PaymentAbortRequest PaymentOrder { get; } = new PaymentAbortRequest();
    }
}