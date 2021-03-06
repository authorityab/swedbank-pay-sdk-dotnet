﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

using SwedbankPay.Sdk.Tests.TestHelpers;

using Xunit.Sdk;

namespace SwedbankPay.Sdk.Tests
{
    public abstract class ResourceTestsBase
    {
        protected SwedbankPayClient Sut;

        protected readonly Urls urls;

        private readonly SwedbankPayConnectionSettings connectionSettings;
        
        public ResourceTestsBase()
        {
            this.connectionSettings = TestHelper.GetSwedbankPayConnectionSettings(Environment.CurrentDirectory);
            this.urls = TestHelper.GetUrls(Environment.CurrentDirectory);
            var client = new HttpClient { BaseAddress = this.connectionSettings.ApiBaseUrl };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.connectionSettings.Token);
            this.Sut = new SwedbankPayClient(client);
        }
    }
}