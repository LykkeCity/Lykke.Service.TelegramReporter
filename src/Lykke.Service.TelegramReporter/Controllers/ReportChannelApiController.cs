using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.TelegramReporter.Client.Api;
using Lykke.Service.TelegramReporter.Client.Models;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.TelegramReporter.Controllers
{
    [Route("api/v1/reportchannel")]
    public class ReportChannelApiController : Controller, IReportChannelApi
    {
        private readonly IChannelManager _channelManager;

        public ReportChannelApiController(IChannelManager channelManager)
        {
            _channelManager = channelManager;
        }

        /// <summary>
        /// Gets all types.
        /// </summary>
        /// <returns></returns>
        [HttpGet("getalltypes")]
        [ProducesResponseType(typeof(IReadOnlyList<string>), (int)HttpStatusCode.OK)]
        public Task<IReadOnlyList<string>> GetAllTypesAsync()
        {
            return _channelManager.GetChanelTypesAsync();
        }

        /// <summary>
        /// Gets all channels.
        /// </summary>
        /// <returns></returns>
        [HttpGet("getall")]
        [ProducesResponseType(typeof(IReadOnlyList<ReportChannelDto>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<ReportChannelDto>> GetAllAsync()
        {
            var data = await _channelManager.GetAllChannelsAsync();
            return data.Select(e => new ReportChannelDto(e.ChannelId, e.Type, e.ChatId, e.Interval, e.Metainfo)).ToList();
        }

        /// <summary>
        /// Adds channel or update exist channel.
        /// </summary>
        /// <param name="channelDto">Channel</param>
        [HttpPost("addorupdate")]
        [ProducesResponseType(typeof(ReportChannelDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ReportChannelDto> AddOrUpdateAsync(ReportChannelDto channelDto)
        {
            var id = channelDto.ChannelId;
            if (string.IsNullOrEmpty(id))
            {
                id = await _channelManager.AddChannelAsync(channelDto.Type, channelDto.ChatId, channelDto.Interval, channelDto.Metainfo);
            }
            else
            {
                await _channelManager.UpdateChannelAsync(id, channelDto.Type, channelDto.ChatId, channelDto.Interval, channelDto.Metainfo);
            }
            return new ReportChannelDto(id, channelDto.Type, channelDto.ChatId, channelDto.Interval, channelDto.Metainfo);
        }

        /// <summary>
        /// Deletes chennel.
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public Task DeleteAsync(string channelId)
        {
            return _channelManager.RemoveChannelAsync(channelId);
        }
    }
}
