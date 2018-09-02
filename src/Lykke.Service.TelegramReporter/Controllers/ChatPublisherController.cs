using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Models;

namespace Lykke.Service.TelegramReporter.Controllers
{
    [Route("api/v1/[controller]")]
    public class ChatPublisherController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IChatPublisherService _chatPublisherService;

        public ChatPublisherController(IMapper mapper, IChatPublisherService chatPublisherService)
        {
            _mapper = mapper;
            _chatPublisherService = chatPublisherService;
        }

        /// <summary>
        /// Gets NE chat publishers.
        /// </summary>
        /// <returns>NE chat publishers</returns>
        [HttpGet("nechatpublishersettings", Name = "GetNeChatPublisherSettingsAsync")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetNeChatPublisherSettingsAsync()
        {
            var chatPublishers = await _chatPublisherService.GetNeChatPublishersAsync();
            var vm = _mapper.Map<IReadOnlyList<IChatPublisherSettings>, IReadOnlyList<ChatPublisherSettingsDto>>(chatPublishers);
            return Ok(vm);
        }

        /// <summary>
        /// Gets balance chat publishers.
        /// </summary>
        /// <returns>Balance chat publishers</returns>
        [HttpGet("balancechatpublishersettings", Name = "GetBalanceChatPublisherSettingsAsync")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBalanceChatPublisherSettingsAsync()
        {
            var chatPublishers = await _chatPublisherService.GetBalanceChatPublishersAsync();
            var vm = _mapper.Map<IReadOnlyList<IChatPublisherSettings>, IReadOnlyList<ChatPublisherSettingsDto>>(chatPublishers);
            return Ok(vm);
        }

        /// <summary>
        /// Gets external balance chat publishers.
        /// </summary>
        /// <returns>External balance chat publishers</returns>
        [HttpGet("externalbalancechatpublishersettings", Name = "GetExternalBalanceChatPublisherSettingsAsync")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExternalBalanceChatPublisherSettingsAsync()
        {
            var chatPublishers = await _chatPublisherService.GetExternalBalanceChatPublishersAsync();
            var vm = _mapper.Map<IReadOnlyList<IChatPublisherSettings>, IReadOnlyList<ChatPublisherSettingsDto>>(chatPublishers);
            return Ok(vm);
        }

        /// <summary>
        /// Adds NE chat publisher.
        /// </summary>
        /// <param name="chatPublisher">NE chat publisher to add.</param>
        [HttpPut("nechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddNeChatPublisherSettingsAsync([FromBody] ChatPublisherSettingsPost chatPublisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.Create(ModelState));
            }

            var model = _mapper
                .Map<ChatPublisherSettings>(chatPublisher);

            try
            {
                await _chatPublisherService.AddNeChatPublisherAsync(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Adds balance chat publisher.
        /// </summary>
        /// <param name="chatPublisher">Balance chat publisher to add.</param>
        [HttpPut("balancechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddBalanceChatPublisherSettingsAsync([FromBody] ChatPublisherSettingsPost chatPublisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.Create(ModelState));
            }

            var model = _mapper
                .Map<ChatPublisherSettings>(chatPublisher);

            try
            {
                await _chatPublisherService.AddBalanceChatPublisherAsync(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Adds external balance chat publisher.
        /// </summary>
        /// <param name="chatPublisher">External balance chat publisher to add.</param>
        [HttpPut("externalbalancechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddExternalBalanceChatPublisherSettingsAsync([FromBody] ChatPublisherSettingsPost chatPublisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.Create(ModelState));
            }

            var model = _mapper
                .Map<ChatPublisherSettings>(chatPublisher);

            try
            {
                await _chatPublisherService.AddExternalBalanceChatPublisherAsync(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes NE chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">NE chat publisher settings Id</param>
        /// <returns></returns>
        [HttpDelete("nechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveNeChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            await _chatPublisherService.RemoveNeChatPublisherAsync(chatPublisherSettingsId);
            return NoContent();
        }

        /// <summary>
        /// Deletes balance chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">Balance chat publisher settings Id</param>
        /// <returns></returns>
        [HttpDelete("balancechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            await _chatPublisherService.RemoveBalanceChatPublisherAsync(chatPublisherSettingsId);
            return NoContent();
        }

        /// <summary>
        /// Deletes external balance chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">External balance chat publisher settings Id</param>
        /// <returns></returns>
        [HttpDelete("externalbalancechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveExternalBalanceChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            await _chatPublisherService.RemoveExternalBalanceChatPublisherAsync(chatPublisherSettingsId);
            return NoContent();
        }

        /// <summary>
        /// Gets balances warnings.
        /// </summary>
        /// <returns>Balances warnings</returns>
        [HttpGet("balanceswarnings", Name = "GetBalancesWarningsAsync")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<BalanceWarningDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBalancesWarningsAsync()
        {
            var balancesWarnings = await _chatPublisherService.GetBalancesWarningsAsync();
            var vm = _mapper.Map<IReadOnlyList<IBalanceWarning>, IReadOnlyList<BalanceWarningDto>>(balancesWarnings);
            return Ok(vm);
        }

        /// <summary>
        /// Gets external balances warnings.
        /// </summary>
        /// <returns>External balances warnings</returns>
        [HttpGet("externalbalanceswarnings", Name = "GetExternalBalancesWarningsAsync")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<ExternalBalanceWarningDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExternalBalancesWarningsAsync()
        {
            var balancesWarnings = await _chatPublisherService.GetExternalBalancesWarningsAsync();
            var vm = _mapper.Map<IReadOnlyList<IExternalBalanceWarning>, IReadOnlyList<ExternalBalanceWarningDto>>(balancesWarnings);
            return Ok(vm);
        }

        /// <summary>
        /// Adds balance warning.
        /// </summary>
        /// <param name="balanceWarning">Balance warning to add.</param>
        [HttpPut("balancewarning")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddBalanceWarningAsync([FromBody] BalanceWarningPost balanceWarning)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.Create(ModelState));
            }

            var model = _mapper
                .Map<BalanceWarning>(balanceWarning);

            try
            {
                await _chatPublisherService.AddBalanceWarningAsync(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Adds external balance warning.
        /// </summary>
        /// <param name="balanceWarning">External balance warning to add.</param>
        [HttpPut("externalbalancewarning")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddExternalBalanceWarningAsync([FromBody] ExternalBalanceWarningPost balanceWarning)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.Create(ModelState));
            }

            var model = _mapper
                .Map<ExternalBalanceWarning>(balanceWarning);

            try
            {
                await _chatPublisherService.AddExternalBalanceWarningAsync(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes balance warning.
        /// </summary>
        /// <param name="clientId">Wallet</param>
        /// <param name="assetId">Asset ID</param>
        /// <returns></returns>
        [HttpDelete("balancewarning")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveBalanceWarningAsync(string clientId, string assetId)
        {
            await _chatPublisherService.RemoveBalanceWarningAsync(clientId, assetId);
            return NoContent();
        }

        /// <summary>
        /// Deletes external balance warning.
        /// </summary>
        /// <param name="exchange">Exchange</param>
        /// <param name="assetId">Asset ID</param>
        /// <returns></returns>
        [HttpDelete("externalbalancewarning")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveExternalBalanceWarningAsync(string exchange, string assetId)
        {
            await _chatPublisherService.RemoveExternalBalanceWarningAsync(exchange, assetId);
            return NoContent();
        }
    }
}
