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

        Task AddNeChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddExternalBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);

        Task RemoveNeChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveBalanceChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveExternalBalanceChatPublisherSettingsAsync(string chatPublisherId);
    }
}
