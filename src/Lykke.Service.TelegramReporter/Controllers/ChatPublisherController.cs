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
        /// Gets CML chat publishers.
        /// </summary>
        /// <returns>CML chat publishers</returns>
        [HttpGet("cmlchatpublishersettings", Name = "GetCmlChatPublisherSettingsAsync")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCmlChatPublisherSettingsAsync()
        {
            var chatPublishers = await _chatPublisherService.GetCmlChatPublishersAsync();
            var vm = _mapper.Map<IReadOnlyList<IChatPublisherSettings>, IReadOnlyList<ChatPublisherSettingsDto>>(chatPublishers);
            return Ok(vm);
        }

        /// <summary>
        /// Gets SE chat publishers.
        /// </summary>
        /// <returns>SE chat publishers</returns>
        [HttpGet("sechatpublishersettings", Name = "GetSeChatPublisherSettingsAsync")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSeChatPublisherSettingsAsync()
        {
            var chatPublishers = await _chatPublisherService.GetSeChatPublishersAsync();
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
        /// Adds CML chat publisher.
        /// </summary>
        /// <param name="chatPublisher">CML chat publisher to add.</param>
        [HttpPut("cmlchatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddCmlChatPublisherSettingsAsync([FromBody] ChatPublisherSettingsPost chatPublisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.Create(ModelState));
            }

            var model = _mapper
                .Map<ChatPublisherSettings>(chatPublisher);

            try
            {
                await _chatPublisherService.AddCmlChatPublisherAsync(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Adds SE chat publisher.
        /// </summary>
        /// <param name="chatPublisher">SE chat publisher to add.</param>
        [HttpPut("sechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddSeChatPublisherSettingsAsync([FromBody] ChatPublisherSettingsPost chatPublisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponse.Create(ModelState));
            }

            var model = _mapper
                .Map<ChatPublisherSettings>(chatPublisher);

            try
            {
                await _chatPublisherService.AddSeChatPublisherAsync(model);
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
        /// Deletes CML chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">CML chat publisher settings Id</param>
        /// <returns></returns>
        [HttpDelete("cmlchatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveCmlChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            await _chatPublisherService.RemoveCmlChatPublisherAsync(chatPublisherSettingsId);
            return NoContent();
        }

        /// <summary>
        /// Deletes SE chat publisher.
        /// </summary>
        /// <param name="chatPublisherSettingsId">SE chat publisher settings Id</param>
        /// <returns></returns>
        [HttpDelete("sechatpublishersettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveSeChatPublisherSettingsAsync(string chatPublisherSettingsId)
        {
            await _chatPublisherService.RemoveSeChatPublisherAsync(chatPublisherSettingsId);
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
        /// Adds balance warning.
        /// </summary>
        /// <param name="balanceWarning">Balance warning to add.</param>
        [HttpPut("balancechatpublishersettings")]
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
    }
}
