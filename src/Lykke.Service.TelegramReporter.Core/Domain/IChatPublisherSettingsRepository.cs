using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Domain
{
    public interface IChatPublisherSettingsRepository
    {
        Task<IReadOnlyList<IChatPublisherSettings>> GetCmlChatPublisherSettings();
        Task<IReadOnlyList<IChatPublisherSettings>> GetSeChatPublisherSettings();

        Task AddCmlChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);
        Task AddSeChatPublisherSettingsAsync(IChatPublisherSettings chatPublisher);

        Task RemoveCmlChatPublisherSettingsAsync(string chatPublisherSettingsId);
        Task RemoveSeChatPublisherSettingsAsync(string chatPublisherId);
    }
}
