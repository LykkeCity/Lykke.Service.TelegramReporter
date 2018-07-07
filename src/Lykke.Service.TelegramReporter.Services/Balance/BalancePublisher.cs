using System;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Balance;

namespace Lykke.Service.TelegramReporter.Services.Balance
{
    public class BalancePublisher : ChatPublisher
    {
        private readonly IBalanceWarningProvider _balanceWarningProvider;

        public BalancePublisher(ITelegramSender telegramSender,
            IBalanceWarningProvider balanceWarningProvider,
            IChatPublisherSettings publisherSettings,
            ILog log)
            : base(telegramSender, publisherSettings, log)
        {
            _balanceWarningProvider = balanceWarningProvider;
        }

        public override async void Publish()
        {
            try
            {
                await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                    await _balanceWarningProvider.GetWarningMessageAsync());
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(BalancePublisher), nameof(Publish), "", ex);
            }
        }
    }
}
