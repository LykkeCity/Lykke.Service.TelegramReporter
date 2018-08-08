using System;
using Autofac;

namespace Lykke.Service.TelegramReporter.Client
{
    public static class AutofacExtension
    {
        public static void RegisterTelegramReporterClient(this ContainerBuilder builder, string serviceUrl)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<TelegramReporterClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ITelegramReporterClient>()
                .SingleInstance();
        }

        public static void RegisterTelegramReporterClient(this ContainerBuilder builder, TelegramReporterServiceClientSettings settings)
        {
            builder.RegisterTelegramReporterClient(settings?.ServiceUrl);
        }
    }
}
