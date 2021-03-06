﻿using Newtonsoft.Json;

using System.Collections.Generic;
using System.Globalization;

namespace SwedbankPay.Sdk.Payments.Swish
{
    public class PaymentRequest
    {
        public PaymentRequest(CurrencyCode currency,
                              List<Price> prices,
                              string description,
                              string payerReference,
                              string userAgent,
                              CultureInfo language,
                              Urls urls,
                              PayeeInfo payeeInfo,
                              PrefillInfo prefillInfo,
                              SwishRequest swishRequest,
                              Dictionary<string, object> metaData = null)
        {
            Payment = new PaymentRequestObject(currency, prices, description, payerReference, userAgent, language, urls, payeeInfo,
                                               prefillInfo, swishRequest, metaData);
        }


        public PaymentRequestObject Payment { get; }

        public class PaymentRequestObject
        {
            protected internal PaymentRequestObject(CurrencyCode currency,
                                                    List<Price> prices,
                                                    string description,
                                                    string payerReference,
                                                    string userAgent,
                                                    CultureInfo language,
                                                    Urls urls,
                                                    PayeeInfo payeeInfo,
                                                    PrefillInfo prefillInfo,
                                                    SwishRequest swishRequest,
                                                    Dictionary<string, object> metaData = null)
            {
                Operation = Operation.Purchase;
                Intent = Intent.Sale;
                Currency = currency;
                Prices = prices;
                Description = description;
                PayerReference = payerReference;
                UserAgent = userAgent;
                Language = language;
                Urls = urls;
                PayeeInfo = payeeInfo;
                PrefillInfo = prefillInfo;
                SwishRequest = swishRequest;
                MetaData = metaData;
            }


            
            public CurrencyCode Currency { get; }
            public string Description { get; }
            public Intent Intent { get; }
            public CultureInfo Language { get; }

            public Operation Operation { get; }
            public PayeeInfo PayeeInfo { get; }
            public string PayerReference { get; }
            public PrefillInfo PrefillInfo { get; }
            public List<Price> Prices { get; }

            [JsonProperty("swish")] public SwishRequest SwishRequest { get; }

            public Urls Urls { get; }
            public string UserAgent { get; }
            public Dictionary<string, object> MetaData { get; }
        }
    }

    public class SwishRequest
    {
        public SwishRequest(bool ecomOnlyEnabled = false)
        {
            EcomOnlyEnabled = ecomOnlyEnabled;
        }


        public bool EcomOnlyEnabled { get; }
    }
}