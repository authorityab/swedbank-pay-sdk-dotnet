﻿namespace SwedbankPay.Sdk.PaymentOrders
{
    using SwedbankPay.Sdk.Consumers;

    /// <summary>
    /// If shipIndicator set to 4, then prefil this.
    /// </summary><see cref="RiskIndicator.ShipIndicator"/>
    public class PickUpAddress
    {
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string CoAddress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public CountryCode CountryCode { get; set; }
    }
}