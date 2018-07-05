using Lykke.Service.TelegramReporter.Core.Services;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using Telegram.Bot.Types;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlSummarySubscriber : ChatSubscriber
    {
        private const string CmlSummaryCommand = "/cmlsummary";

        private readonly ICmlSummaryProvider _cmlSummaryProvider;

        public CmlSummarySubscriber(ICmlSummaryProvider cmlSummaryProvider,
            IChatPublisherSettings publisherSettings)
            : base(publisherSettings)
        {
            _cmlSummaryProvider = cmlSummaryProvider;
        }

        public override string Command => CmlSummaryCommand;

        public override async Task ProcessMessageInternalAsync(ITelegramSender telegramSender, Message message)
        {
            await telegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                await _cmlSummaryProvider.GetSummaryMessageAsync(),
                replyToMessageId: message.MessageId);
        }

        public override Task ProcessCallbackQueryInternal(ITelegramSender telegramSender, CallbackQuery callbackQuery)
        {
            return Task.CompletedTask;
        }
    }
}
