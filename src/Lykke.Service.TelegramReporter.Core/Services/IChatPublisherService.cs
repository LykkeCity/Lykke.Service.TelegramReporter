using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;

namespace Lykke.Service.TelegramReporter.Core.Services
{
    public interface IChatPublisherService
    {
        Task<IReadOnlyList<IChatPublisherSettings>> GetCmlChatPublishers();
        Task<IReadOnlyList<IChatPublisherSettings>> GetSeChatPublishers();
        Task<IReadOnlyList<IChatPublisherSettings>> GetBalanceChatPublishers();
        Task AddCmlChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddSeChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task AddBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher);
        Task RemoveCmlChatPublisherAsync(string chatPublisherId);
        Task RemoveSeChatPublisherAsync(string chatPublisherId);
        Task RemoveBalanceChatPublisherAsync(string chatPublisherId);
    }
}
