﻿namespace SwedbankPay.Sdk
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SwedbankPay.Sdk.PaymentOrders;

    using System;
    using System.Linq;

    public class CustomEmailAddressConverter : JsonConverter

    {
        private readonly Type[] types;

        public CustomEmailAddressConverter(params Type[] types)
        {
            this.types = types;
        }

        public CustomEmailAddressConverter()
        {

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                var o = (JObject)t;
                writer.WriteValue(value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var addressString = jo.Values().FirstOrDefault()?.ToString();
            var address = new EmailAddress(addressString);
            return address;
        }

        public override bool CanRead => true;


        public override bool CanConvert(Type objectType)
        {
            return this.types.Any(t => t == objectType);
        }
    }
}