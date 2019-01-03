using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Core.Services.Channelv2
{
    public interface IChannelRepository
    {
        Task AddOrUpdateChannelAsync(IReportChannel channel);
        Task RemoveChannelAsync(string channelId);
        Task<List<IReportChannel>> GetAllChannelsAsync();
    }
}
