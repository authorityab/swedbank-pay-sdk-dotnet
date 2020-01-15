﻿using Atata;
using NUnit.Framework;
using Sample.AspNetCore.SystemTests.Services;
using Sample.AspNetCore.SystemTests.Test.Helpers;
using SwedbankPay.Sdk;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.AspNetCore.SystemTests.Test.PaymentTests.Payment
{
    public class PaymentCaptureTests : Base.PaymentTests
    {
        public PaymentCaptureTests(string driverAlias)
            : base(driverAlias)
        {
        }


        [Test]
        [TestCaseSource(nameof(TestData), new object[] { false, PaymentMethods.Card })]
        public async Task Payment_Card_Capture(Product[] products, PayexInfo payexInfo)
        {
            GoToOrdersPage(products, payexInfo, Checkout.Option.LocalPaymentMenu)
                .PaymentLink.StoreValue(out var paymentLink)
                .Actions.Rows[y => y.Name.Value.Contains(PaymentResourceOperations.CreateCapture)].ExecuteAction.ClickAndGo()
                .Actions.Rows[y => y.Name.Value.Contains(PaymentResourceOperations.CreateReversal)].Should.BeVisible()
                .Actions.Rows[y => y.Name.Value.Contains(PaymentResourceOperations.PaidPayment)].Should.BeVisible()
                .Actions.Rows.Count.Should.Equal(2);

            var cardPayment = await SwedbankPayClient.Payment.GetCreditCardPayment(paymentLink, SwedbankPay.Sdk.Payments.PaymentExpand.All);

            // Operations
            Assert.That(cardPayment.Operations[LinkRelation.CreatePaymentOrderCancel], Is.Null);
            Assert.That(cardPayment.Operations[LinkRelation.CreatePaymentOrderCapture], Is.Null);
            Assert.That(cardPayment.Operations[LinkRelation.PaidPaymentOrder], Is.Not.Null);
            Assert.That(cardPayment.Operations[LinkRelation.CreatePaymentOrderReversal], Is.Not.Null);

            // Transactions
            Assert.That(cardPayment.PaymentResponse.Transactions.TransactionList.Count, Is.EqualTo(2));
            Assert.That(cardPayment.PaymentResponse.Transactions.TransactionList.First(x => x.Type == TransactionTypes.Authorization).State,
                        Is.EqualTo(State.Completed));
            Assert.That(cardPayment.PaymentResponse.Transactions.TransactionList.First(x => x.Type == TransactionTypes.Capture).State,
                        Is.EqualTo(State.Completed));
        }

    }
}
