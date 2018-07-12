using System;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEnginePublisher : ChatPublisher
    {
        private readonly INettingEngineStateProvider _nettingEngineStateProvider;

        public NettingEnginePublisher(ITelegramSender telegramSender,
            INettingEngineStateProvider nettingEngineStateProvider,
            IChatPublisherSettings publisherSettings, ILog log)
            : base(telegramSender, publisherSettings, log)
        {
            _nettingEngineStateProvider = nettingEngineStateProvider;
        }

        public override async void Publish()
        {
            try
            {
                await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                    await _nettingEngineStateProvider.GetStateMessageAsync());
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(NettingEnginePublisher), nameof(Publish), "", ex);
            }
        }
    }
}
