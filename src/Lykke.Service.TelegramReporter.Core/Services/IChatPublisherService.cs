using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Services
{
    public interface IChatPublisherService
    {
        Task<IReadOnlyList<IChatPublisherSettings>> GetCmlChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetSeChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetNeChatPublishersAsync();
        Task<IReadOnlyList<IChatPublisherSettings>> GetBalanceChatPublishersAsync();
        Task AddCmlChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddSeChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddNeChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task RemoveCmlChatPublisherAsync(string chatPublisherId);
        Task RemoveSeChatPublisherAsync(string chatPublisherId);
        Task RemoveNeChatPublisherAsync(string chatPublisherId);
        Task RemoveBalanceChatPublisherAsync(string chatPublisherId);

        Task<IReadOnlyList<IBalanceWarning>> GetBalancesWarningsAsync();
        Task AddBalanceWarningAsync(IBalanceWarning balanceWarning);
        Task RemoveBalanceWarningAsync(string clientId, string assetId);
    }
}
