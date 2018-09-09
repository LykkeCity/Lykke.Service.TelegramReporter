using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Services
{
    public interface IChatPublisherService
    {
        Task<IReadOnlyList<IChatPublisherSettings>> GetNeChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetBalanceChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetExternalBalanceChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetWalletsRebalancerChatPublishersAsync();
        Task AddNeChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddExternalBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddWalletsRebalancerChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task RemoveNeChatPublisherAsync(string chatPublisherId);
        Task RemoveBalanceChatPublisherAsync(string chatPublisherId);
        Task RemoveExternalBalanceChatPublisherAsync(string chatPublisherId);
        Task RemoveWalletsRebalancerChatPublisherAsync(string chatPublisherId);

        Task<IReadOnlyList<IBalanceWarning>> GetBalancesWarningsAsync();
        Task<IReadOnlyList<IExternalBalanceWarning>> GetExternalBalancesWarningsAsync();
        Task AddBalanceWarningAsync(IBalanceWarning balanceWarning);
        Task AddExternalBalanceWarningAsync(IExternalBalanceWarning balanceWarning);
        Task RemoveBalanceWarningAsync(string clientId, string assetId);
        Task RemoveExternalBalanceWarningAsync(string exchange, string assetId);

        ConcurrentDictionary<long, ChatPublisher> NePublishers { get; }
    }
}
