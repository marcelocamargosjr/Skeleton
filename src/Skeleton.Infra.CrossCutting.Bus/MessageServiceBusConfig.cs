﻿namespace Skeleton.Infra.CrossCutting.Bus
{
    public class MessageServiceBusConfig
    {
        public string AzureServiceBusConnectionString { get; set; }
        public string SendinblueApiKey { get; set; }
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
    }
}