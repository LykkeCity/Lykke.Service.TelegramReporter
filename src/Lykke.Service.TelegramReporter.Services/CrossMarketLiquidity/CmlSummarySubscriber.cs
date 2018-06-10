using Lykke.Service.TelegramReporter.Core.Services;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using Telegram.Bot.Types;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlSummarySubscriber : ITelegramSubscriber
    {
        private const string CmlSummaryCommand = "/cmlsummary";

        private readonly ICmlSummaryProvider _cmlSummaryProvider;

        public CmlSummarySubscriber(ICmlSummaryProvider cmlSummaryProvider)
        {
            _cmlSummaryProvider = cmlSummaryProvider;
        }

        public string Command => CmlSummaryCommand;

        public async Task ProcessMessageAsync(ITelegramSender telegramSender, Message message)
        {
            await telegramSender.SendTextMessageAsync(message.Chat.Id,
                await _cmlSummaryProvider.GetSummaryMessageAsync(),
                replyToMessageId: message.MessageId);
        }

        public Task ProcessCallbackQuery(ITelegramSender telegramSender, CallbackQuery callbackQuery)
        {
            return Task.CompletedTask;
        }
    }
}
