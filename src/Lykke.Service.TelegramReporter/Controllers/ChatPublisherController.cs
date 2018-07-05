﻿using System;
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
        [HttpGet("cmlchatpublishersettings", Name = "GetCmlChatPublisherSettings")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCmlChatPublisherSettings()
        {
            var chatPublishers = await _chatPublisherService.GetCmlChatPublishers();
            var vm = _mapper.Map<IReadOnlyList<IChatPublisherSettings>, IReadOnlyList<ChatPublisherSettingsDto>>(chatPublishers);
            return Ok(vm);
        }

        /// <summary>
        /// Gets SE chat publishers.
        /// </summary>
        /// <returns>SE chat publishers</returns>
        [HttpGet("sechatpublishersettings", Name = "GetSeChatPublisherSettings")]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IReadOnlyList<ChatPublisherSettingsDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSeChatPublisherSettings()
        {
            var chatPublishers = await _chatPublisherService.GetSeChatPublishers();
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
    }
}
