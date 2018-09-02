using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Client.Api;
using Lykke.Service.TelegramReporter.Client.Models;

namespace Lykke.Service.TelegramReporter.Controllers
{
    [Route("api/v1/[controller]")]
    public class ChatPublisherController : Controller, ITelegramReporterApi
    {
        private readonly IChatPublisherService _chatPublisherService;

        public ChatPublisherController(IChatPublisherService chatPublisherService)
        {
            _chatPublisherService = chatPublisherService;
        }

        /// <summary>
        /// Gets NE chat publishers.
        /// </summary>
        /// <returns>NE chat publishers</returns>
        [HttpGet("nechatpublishersettings")]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<ChatPublisherSettingsDto>> GetNeChatPublisherSettingsAsync()
        {
            var chatPublishers = await _chatPublisherService.GetNeChatPublishersAsync();
            var vm = Mapper.Map<IReadOnlyList<IChatPublisherSettings>, IReadOnlyList<ChatPublisherSettingsDto>>(chatPublishers);
            return vm;
        }

        /// <summary>
        /// Gets balance chat publishers.
        /// </summary>
        /// <returns>Balance chat publishers</returns>
        [HttpGet("balancechatpublishersettings")]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<ChatPublisherSettingsDto>> GetBalanceChatPublisherSettingsAsync()
        {
            var chatPublishers = await _chatPublisherService.GetBalanceChatPublishersAsync();
            var vm = Mapper.Map<IReadOnlyList<IChatPublisherSettings>, IReadOnlyList<ChatPublisherSettingsDto>>(chatPublishers);
            return vm;
        }

        /// <summary>
        /// Gets external balance chat publishers.
        /// </summary>
        /// <returns>External balance chat publishers</returns>
        [HttpGet("externalbalancechatpublishersettings")]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<ChatPublisherSettingsDto>> GetExternalBalanceChatPublisherSettingsAsync()
        {
            var chatPublishers = await _chatPublisherService.GetExternalBalanceChatPublishersAsync();
            var vm = Mapper.Map<IReadOnlyList<IChatPublisherSettings>, IReadOnlyList<ChatPublisherSettingsDto>>(chatPublishers);
            return vm;
        }

        /// <summary>
        /// Adds NE chat publisher.
        /// </summary>
        /// <param name="chatPublisher">NE chat publisher to add.</param>
        [HttpPost("nechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task AddNeChatPublisherSettingsAsync([FromBody] ChatPublisherSettingsPost chatPublisher)
        {
            var model = Mapper.Map<ChatPublisherSettings>(chatPublisher);

            await _chatPublisherService.AddNeChatPublisherAsync(model);
        }

        /// <summary>
        /// Adds balance chat publisher.
        /// </summary>
        /// <param name="chatPublisher">Balance chat publisher to add.</param>
        [HttpPost("balancechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task AddBalanceChatPublisherSettingsAsync([FromBody] ChatPublisherSettingsPost chatPublisher)
        {
            var model = Mapper.Map<ChatPublisherSettings>(chatPublisher);

            await _chatPublisherService.AddBalanceChatPublisherAsync(model);
        }

        /// <summary>
        /// Adds external balance chat publisher.
        /// </summary>
        /// <param name="chatPublisher">External balance chat publisher to add.</param>
        [HttpPost("externalbalancechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task AddExternalBalanceChatPublisherSettingsAsync([FromBody] ChatPublisherSettingsPost chatPublisher)
        {
            var model = Mapper.Map<ChatPublisherSettings>(chatPublisher);

            await _chatPublisherService.AddExternalBalanceChatPublisherAsync(model);
        }

        /// <summary>
        /// Deletes NE chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">NE chat publisher settings Id</param>
        /// <returns></returns>
        [HttpDelete("nechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task RemoveNeChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            if (string.IsNullOrEmpty(chatPublisherSettingsId))
            {
                throw new ValidationApiException($"{nameof(chatPublisherSettingsId)} required");
            }

            await _chatPublisherService.RemoveNeChatPublisherAsync(chatPublisherSettingsId);
        }

        /// <summary>
        /// Deletes balance chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">Balance chat publisher settings Id</param>
        /// <returns></returns>
        [HttpDelete("balancechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task RemoveBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            if (string.IsNullOrEmpty(chatPublisherSettingsId))
            {
                throw new ValidationApiException($"{nameof(chatPublisherSettingsId)} required");
            }

            await _chatPublisherService.RemoveBalanceChatPublisherAsync(chatPublisherSettingsId);
        }

        /// <summary>
        /// Deletes external balance chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">External balance chat publisher settings Id</param>
        /// <returns></returns>
        [HttpDelete("externalbalancechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task RemoveExternalBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            if (string.IsNullOrEmpty(chatPublisherSettingsId))
            {
                throw new ValidationApiException($"{nameof(chatPublisherSettingsId)} required");
            }

            await _chatPublisherService.RemoveExternalBalanceChatPublisherAsync(chatPublisherSettingsId);
        }

        /// <summary>
        /// Gets balances warnings.
        /// </summary>
        /// <returns>Balances warnings</returns>
        [HttpGet("balanceswarnings")]
        [ProducesResponseType(typeof(IReadOnlyList<BalanceWarningDto>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<BalanceWarningDto>> GetBalancesWarningsAsync()
        {
            var balancesWarnings = await _chatPublisherService.GetBalancesWarningsAsync();
            var vm = Mapper.Map<IReadOnlyList<IBalanceWarning>, IReadOnlyList<BalanceWarningDto>>(balancesWarnings);
            return vm;
        }

        /// <summary>
        /// Gets external balances warnings.
        /// </summary>
        /// <returns>External balances warnings</returns>
        [HttpGet("externalbalanceswarnings")]
        [ProducesResponseType(typeof(IReadOnlyList<ExternalBalanceWarningDto>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<ExternalBalanceWarningDto>> GetExternalBalancesWarningsAsync()
        {
            var balancesWarnings = await _chatPublisherService.GetExternalBalancesWarningsAsync();
            var vm = Mapper.Map<IReadOnlyList<IExternalBalanceWarning>, IReadOnlyList<ExternalBalanceWarningDto>>(balancesWarnings);
            return vm;
        }

        /// <summary>
        /// Adds balance warning.
        /// </summary>
        /// <param name="balanceWarning">Balance warning to add.</param>
        [HttpPost("balancewarning")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task AddBalanceWarningAsync([FromBody] BalanceWarningPost balanceWarning)
        {
            var model = Mapper.Map<BalanceWarning>(balanceWarning);

            await _chatPublisherService.AddBalanceWarningAsync(model);
        }

        /// <summary>
        /// Adds external balance warning.
        /// </summary>
        /// <param name="balanceWarning">External balance warning to add.</param>
        [HttpPost("externalbalancewarning")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task AddExternalBalanceWarningAsync([FromBody] ExternalBalanceWarningPost balanceWarning)
        {
            var model = Mapper.Map<ExternalBalanceWarning>(balanceWarning);

            await _chatPublisherService.AddExternalBalanceWarningAsync(model);
        }

        /// <summary>
        /// Deletes balance warning.
        /// </summary>
        /// <param name="clientId">Wallet</param>
        /// <param name="assetId">Asset ID</param>
        /// <returns></returns>
        [HttpDelete("balancewarning")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task RemoveBalanceWarningAsync(string clientId, string assetId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ValidationApiException($"{nameof(clientId)} required");
            }

            if (string.IsNullOrEmpty(assetId))
            {
                throw new ValidationApiException($"{nameof(assetId)} required");
            }

            await _chatPublisherService.RemoveBalanceWarningAsync(clientId, assetId);
        }

        /// <summary>
        /// Deletes external balance warning.
        /// </summary>
        /// <param name="exchange">Exchange</param>
        /// <param name="assetId">Asset ID</param>
        /// <returns></returns>
        [HttpDelete("externalbalancewarning")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task RemoveExternalBalanceWarningAsync(string exchange, string assetId)
        {
            if (string.IsNullOrEmpty(exchange))
            {
                throw new ValidationApiException($"{nameof(exchange)} required");
            }

            if (string.IsNullOrEmpty(assetId))
            {
                throw new ValidationApiException($"{nameof(assetId)} required");
            }

            await _chatPublisherService.RemoveExternalBalanceWarningAsync(exchange, assetId);
        }
    }
}
