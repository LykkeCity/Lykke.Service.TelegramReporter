using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Domain
{
    public interface IChatPublisherSettingsRepository
    {
        Task<IReadOnlyList<IChatPublisherSettings>> GetNeChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetBalanceChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetExternalBalanceChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetWalletsRebalancerChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetMarketMakerArbitragesChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetLiquidityEngineTradesChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetLiquidityEngineSummaryChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetNeTradesChatPublisherSettings();

        Task AddNeChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddExternalBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddWalletsRebalancerChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddMarketMakerArbitragesChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddLiquidityEngineTradesChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddLiquidityEngineSummaryChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddNeTradesChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);

        Task RemoveNeChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveBalanceChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveExternalBalanceChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveWalletsRebalancerChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveMarketMakerArbitragesChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveLiquidityEngineTradesChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveLiquidityEngineSummaryChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveNeTradesChatPublisherSettingsAsync(string chatPublisherId);
    }
}
