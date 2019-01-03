using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.TelegramReporter.Client.Models;
using Refit;

namespace Lykke.Service.TelegramReporter.Client.Api
{
    [PublicAPI]
    public interface IReportChannelApi
    {
        [Get("/api/v1/reportchannel/getalltypes")]
        Task<IReadOnlyList<string>> GetAllTypesAsync();

        [Get("/api/v1/reportchannel/getall")]
        Task<IReadOnlyList<ReportChannelDto>> GetAllAsync();

        [Post("/api/v1/reportchannel/addorupdate")]
        Task<ReportChannelDto> AddOrUpdateAsync(ReportChannelDto channelDto);

        [Delete("/api/v1/reportchannel/delete")]
        Task DeleteAsync(string channelId);
    }
}
