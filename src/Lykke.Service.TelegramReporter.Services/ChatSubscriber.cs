using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Services;
using Telegram.Bot.Types;

namespace Lykke.Service.TelegramReporter.Services
{
    public abstract class ChatSubscriber : ITelegramSubscriber
    {
        protected readonly IChatPublisherSettingsRepository _repo;

        protected readonly ILog Log;

        protected ChatSubscriber(IChatPublisherSettingsRepository repo, ILog log)
        {
            _repo = repo;
            Log = log;
        }

        public abstract string Command { get; }

        public async Task ProcessMessageAsync(ITelegramSender telegramSender, Message message)
        {
            if (!await ValidateChat(message.Chat.Id, telegramSender))
            {
                return;
            }

            await ProcessMessageInternalAsync(telegramSender, message);
        }

        public async Task ProcessCallbackQuery(ITelegramSender telegramSender, CallbackQuery callbackQuery)
        {
            if (!await ValidateChat(callbackQuery.Message.Chat.Id, telegramSender))
            {
                return;
            }

            await ProcessCallbackQueryInternal(telegramSender, callbackQuery);
        }

        public abstract Task ProcessMessageInternalAsync(ITelegramSender telegramSender, Message message);
        public abstract Task ProcessCallbackQueryInternal(ITelegramSender telegramSender, CallbackQuery callbackQuery);

        private async Task<bool> ValidateChat(long chatId, ITelegramSender telegramSender)
        {
            var allowedChatIds = await GetAllowedChatIds();

            if (!allowedChatIds.Contains(chatId))
            {
                await telegramSender.SendTextMessageAsync(chatId, "Unrecognized chat");

                return false;
            }

            return true;
        }

        protected abstract Task<long[]> GetAllowedChatIds();
    }
}
