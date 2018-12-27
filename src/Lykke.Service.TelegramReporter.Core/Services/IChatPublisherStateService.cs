using System.Collections.Concurrent;

namespace Lykke.Service.TelegramReporter.Core.Services
{
    public interface IChatPublisherStateService
    {
        ConcurrentDictionary<long, ChatPublisher> NePublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> NeTradesPublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> BalancePublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> ExternalBalancePublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> MarketMakerArbitragesPublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> LiquidityEngineTradesPublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> LiquidityEngineSummaryPublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> CryptoIndexWarningsPublishers { get; }
    }
}
