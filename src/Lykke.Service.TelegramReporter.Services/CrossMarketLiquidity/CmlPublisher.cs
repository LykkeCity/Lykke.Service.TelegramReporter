using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlPublisher : ChatPublisher
    {
        private readonly ICmlSummaryProvider _cmlSummaryProvider;
        private readonly ICmlStateProvider _cmlStateProvider;

        public CmlPublisher(ITelegramSender telegramSender,
            ICmlSummaryProvider cmlSummaryProvider,
            ICmlStateProvider cmlStateProvider,
            IChatPublisherSettings publisherSettings)
            : base(telegramSender, publisherSettings)
        {
            _cmlSummaryProvider = cmlSummaryProvider;
            _cmlStateProvider = cmlStateProvider;
        }

        public override async void Publish()
        {
            await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                await _cmlSummaryProvider.GetSummaryMessageAsync());

            await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                await _cmlStateProvider.GetStateMessageAsync());
        }
    }
}
