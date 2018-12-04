using Lykke.Service.TelegramReporter.Core.Services;
using System.Collections.Concurrent;
using Lykke.Service.TelegramReporter.Core;

namespace Lykke.Service.TelegramReporter.Services
{
    public class ChatPublisherStateService : IChatPublisherStateService
    {
        public ConcurrentDictionary<long, ChatPublisher> NePublishers { get; } =
            new ConcurrentDictionary<long, ChatPublisher>();
        public ConcurrentDictionary<long, ChatPublisher> BalancePublishers { get; } =
            new ConcurrentDictionary<long, ChatPublisher>();
        public ConcurrentDictionary<long, ChatPublisher> ExternalBalancePublishers { get; } =
            new ConcurrentDictionary<long, ChatPublisher>();
        public ConcurrentDictionary<long, ChatPublisher> MarketMakerArbitragesPublishers { get; } =
            new ConcurrentDictionary<long, ChatPublisher>();
        public ConcurrentDictionary<long, ChatPublisher> LiquidityEngineTradesPublishers { get; } =
            new ConcurrentDictionary<long, ChatPublisher>();
        public ConcurrentDictionary<long, ChatPublisher> LiquidityEngineSummaryPublishers { get; } =
            new ConcurrentDictionary<long, ChatPublisher>();

        public ChatPublisherStateService()
        {
        }        
    }
}
