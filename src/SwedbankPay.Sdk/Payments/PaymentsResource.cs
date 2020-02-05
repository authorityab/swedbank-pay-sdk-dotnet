﻿using System;
using System.Threading.Tasks;

using SwedbankPay.Sdk.Payments.Card;

namespace SwedbankPay.Sdk.Payments
{
    internal class PaymentsResource : ResourceBase, IPaymentsResource
    {
        public PaymentsResource(SwedbankPayHttpClient swedbankPayHttpClient)
            : base(swedbankPayHttpClient)
        {
        }


        public async Task<Payment> GetCreditCardPayment(Uri id, PaymentExpand paymentExpand = PaymentExpand.None)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await Payment.Get(id, this.swedbankPayHttpClient, GetExpandQueryString(paymentExpand));
        }


        public async Task<Payment> CreateCreditCardPayment(PaymentRequest paymentRequest, PaymentExpand paymentExpand = PaymentExpand.None)
        {
            return await Payment.Create(paymentRequest, this.swedbankPayHttpClient, GetExpandQueryString(paymentExpand));
        }


        public async Task<Swish.Payment> GetSwishPayment(Uri id, PaymentExpand paymentExpand = PaymentExpand.None)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await Swish.Payment.Get(id, this.swedbankPayHttpClient, GetExpandQueryString(paymentExpand));
        }


        public async Task<Swish.Payment> CreateSwishPayment(Swish.PaymentRequest paymentRequest,
                                                            PaymentExpand paymentExpand = PaymentExpand.None)
        {
            return await Swish.Payment.Create(paymentRequest, this.swedbankPayHttpClient, GetExpandQueryString(paymentExpand));
        }


        public async Task<Vipps.Payment> GetVippsPayment(Uri id, PaymentExpand paymentExpand = PaymentExpand.None)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await Vipps.Payment.Get(id, this.swedbankPayHttpClient, GetExpandQueryString(paymentExpand));
        }


        public async Task<Vipps.Payment> CreateVippsPayment(Vipps.PaymentRequest paymentRequest,
                                                            PaymentExpand paymentExpand = PaymentExpand.None)
        {
            return await Vipps.Payment.Create(paymentRequest, this.swedbankPayHttpClient, GetExpandQueryString(paymentExpand));
        }
    }
}