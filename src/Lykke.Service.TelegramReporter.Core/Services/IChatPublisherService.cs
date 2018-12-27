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
        Task<IReadOnlyList<IChatPublisherSettings>> GetMarketMakerArbitragesChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetLiquidityEngineTradesChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetLiquidityEngineSummaryChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetNeTradesChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetCryptoIndexWarningsChatPublishersAsync();

        Task AddNeChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddExternalBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddWalletsRebalancerChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddMarketMakerArbitragesChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddLiquidityEngineTradesChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddLiquidityEngineSummaryChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddNeTradesChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddCryptoIndexWarningsChatPublisherAsync(IChatPublisherSettings chatPublisher);

        Task RemoveNeChatPublisherAsync(string chatPublisherId);
        Task RemoveBalanceChatPublisherAsync(string chatPublisherId);
        Task RemoveExternalBalanceChatPublisherAsync(string chatPublisherId);
        Task RemoveWalletsRebalancerChatPublisherAsync(string chatPublisherId);
        Task RemoveMarketMakerArbitragesChatPublisherAsync(string chatPublisherId);
        Task RemoveLiquidityEngineTradesChatPublisherAsync(string chatPublisherId);
        Task RemoveLiquidityEngineSummaryChatPublisherAsync(string chatPublisherId);
        Task RemoveNeTradesChatPublisherAsync(string chatPublisherId);
        Task RemoveCryptoIndexWarningsChatPublisherAsync(string chatPublisherId);

        Task<IReadOnlyList<IBalanceWarning>> GetBalancesWarningsAsync();
        Task<IReadOnlyList<IExternalBalanceWarning>> GetExternalBalancesWarningsAsync();
        Task AddBalanceWarningAsync(IBalanceWarning balanceWarning);
        Task AddExternalBalanceWarningAsync(IExternalBalanceWarning balanceWarning);
        Task RemoveBalanceWarningAsync(string clientId, string assetId);
        Task RemoveExternalBalanceWarningAsync(string exchange, string assetId);

    }
}
