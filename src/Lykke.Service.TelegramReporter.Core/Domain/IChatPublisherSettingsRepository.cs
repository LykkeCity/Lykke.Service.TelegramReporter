using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Domain
{
    public interface IChatPublisherSettingsRepository
    {
        Task<IReadOnlyList<IChatPublisherSettings>> GetCmlChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetSeChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetNeChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetBalanceChatPublisherSettings();

        Task AddCmlChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddSeChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddNeChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddBalanceChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);

        Task RemoveCmlChatPublisherSettingsAsync(string chatPublisherSettingsId);
        Task RemoveSeChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveNeChatPublisherSettingsAsync(string chatPublisherId);
        Task RemoveBalanceChatPublisherSettingsAsync(string chatPublisherId);
    }
}
