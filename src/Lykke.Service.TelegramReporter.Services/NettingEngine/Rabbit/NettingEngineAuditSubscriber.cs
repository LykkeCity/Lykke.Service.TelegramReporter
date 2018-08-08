using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.NettingEngine.Client.RabbitMq;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Core.Settings;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Rabbit
{
    public class NettingEngineAuditSubscriber : IStartable, IStopable
    {
        private readonly NettingEngineAuditExchangeSettings _settings;
        private readonly INettingEngineAuditPublisher _handler;
        private readonly ILog _log;
        private RabbitMqSubscriber<AuditMessage> _subscriber;

        public NettingEngineAuditSubscriber(
            NettingEngineAuditExchangeSettings settings,
            INettingEngineAuditPublisher handler,
            ILog log)
        {
            _settings = settings;
            _handler = handler;
            _log = log;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings.CreateForSubscriber(
                _settings.Exchange.ConnectionString, _settings.Exchange.Exchange, _settings.Exchange.QueueSuffix);
            settings.IsDurable = false;
            settings.DeadLetterExchangeName = null;
            settings.ExchangeName = _settings.Exchange.Exchange;

            _subscriber = new RabbitMqSubscriber<AuditMessage>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<AuditMessage>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private async Task ProcessMessageAsync(AuditMessage auditMessage)
        {
            try
            {
                await _handler.Publish(auditMessage);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(NettingEngineAuditSubscriber), nameof(ProcessMessageAsync), $"auditMessage {auditMessage.ToJson()}", ex);
            }
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}
