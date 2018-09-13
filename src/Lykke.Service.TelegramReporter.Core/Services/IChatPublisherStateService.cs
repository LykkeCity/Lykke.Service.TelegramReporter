using System.Collections.Concurrent;

namespace Lykke.Service.TelegramReporter.Core.Services
{
    public interface IChatPublisherStateService
    {
        ConcurrentDictionary<long, ChatPublisher> NePublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> BalancePublishers { get; }
        ConcurrentDictionary<long, ChatPublisher> ExternalBalancePublishers { get; }
    }
}
