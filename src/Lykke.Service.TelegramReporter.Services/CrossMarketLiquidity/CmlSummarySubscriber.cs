using System;
using System.Linq;
using Lykke.Service.TelegramReporter.Core.Services;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using Telegram.Bot.Types;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlSummarySubscriber : ChatSubscriber
    {
        private const string CmlSummaryCommand = "/cmlsummary";

        private readonly ICmlSummaryProvider _cmlSummaryProvider;

        public CmlSummarySubscriber(ICmlSummaryProvider cmlSummaryProvider,
            IChatPublisherSettingsRepository repo, ILog log)
            : base(repo, log)
        {
            _cmlSummaryProvider = cmlSummaryProvider;
        }

        public override string Command => CmlSummaryCommand;

        public override async Task ProcessMessageInternalAsync(ITelegramSender telegramSender, Message message)
        {
            try
            {
                var allowedChatIds = await GetAllowedChatIds();
                if (allowedChatIds.Contains(message.Chat.Id))
                {
                    await telegramSender.SendTextMessageAsync(message.Chat.Id,
                        await _cmlSummaryProvider.GetSummaryMessageAsync(),
                        replyToMessageId: message.MessageId);
                }
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(CmlSummarySubscriber), nameof(ProcessMessageInternalAsync), "", ex);
            }
        }

        public override Task ProcessCallbackQueryInternal(ITelegramSender telegramSender, CallbackQuery callbackQuery)
        {
            return Task.CompletedTask;
        }

        protected override async Task<long[]> GetAllowedChatIds()
        {
            return (await _repo.GetCmlChatPublisherSettings()).Select(x => x.ChatId).ToArray();
        }
    }
}
