using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.TelegramReporter.Client
{
    public static class AutofacExtension
    {
        public static void RegisterTelegramReporterClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<TelegramReporterClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ITelegramReporterClient>()
                .SingleInstance();
        }

        public static void RegisterTelegramReporterClient(this ContainerBuilder builder, TelegramReporterServiceClientSettings settings, ILog log)
        {
            builder.RegisterTelegramReporterClient(settings?.ServiceUrl, log);
        }
    }
}
