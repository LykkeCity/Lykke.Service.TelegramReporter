using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.MarketMakerWalletsRebalancer.Contract;
using Lykke.Service.TelegramReporter.Core.Services.WalletsRebalancer;
using Lykke.Service.TelegramReporter.Core.Settings;

namespace Lykke.Service.TelegramReporter.Services.WalletsRebalancer.Rabbit
{
    public class WalletsRebalancerSubscriber : IStartable, IStopable
    {
        private readonly WalletsRebalancerExchangeSettings _settings;
        private readonly IWalletsRebalancerPublisher _handler;
        private readonly ILog _log;
        private RabbitMqSubscriber<RebalanceOperation> _subscriber;

        public WalletsRebalancerSubscriber(
            WalletsRebalancerExchangeSettings settings,
            IWalletsRebalancerPublisher handler,
            ILogFactory logFactory)
        {
            _settings = settings;
            _handler = handler;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings.CreateForSubscriber(
                _settings.Exchange.ConnectionString, _settings.Exchange.Exchange, _settings.Exchange.QueueSuffix);
            settings.IsDurable = false;
            settings.DeadLetterExchangeName = null;
            settings.ExchangeName = _settings.Exchange.Exchange;

            _subscriber = new RabbitMqSubscriber<RebalanceOperation>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<RebalanceOperation>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private async Task ProcessMessageAsync(RebalanceOperation message)
        {
            try
            {
                await _handler.Publish(message);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsRebalancerSubscriber), nameof(ProcessMessageAsync), $"message {message.ToJson()}", ex);
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
