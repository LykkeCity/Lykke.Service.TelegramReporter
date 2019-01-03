using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.Channelv2
{
    public interface IChannelManager
    {
        Task<IReadOnlyList<string>> GetChanelTypesAsync();
        Task<string> AddChannelAsync(string type, long chatId, TimeSpan interval, string metainfo);
        Task<string> UpdateChannelAsync(string channelId, string type, long chatId, TimeSpan interval, string metainfo);
        Task RemoveChannelAsync(string channelId);
        Task<IReadOnlyList<IReportChannel>> GetAllChannelsAsync();
    }
}
