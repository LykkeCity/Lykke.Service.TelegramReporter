﻿using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.SpreadEngine;

namespace Lykke.Service.TelegramReporter.Services.SpreadEngine
{
    public class SpreadEnginePublisher : ChatPublisher
    {
        private readonly ISpreadEngineStateProvider _spreadEngineStateProvider;

        public SpreadEnginePublisher(ITelegramSender telegramSender,
            ISpreadEngineStateProvider spreadEngineStateProvider,
            IChatPublisherSettings publisherSettings)
            : base(telegramSender, publisherSettings)
        {
            _spreadEngineStateProvider = spreadEngineStateProvider;
        }

        public override async void Publish()
        {
            await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                await _spreadEngineStateProvider.GetStateMessageAsync());
        }
    }
}
