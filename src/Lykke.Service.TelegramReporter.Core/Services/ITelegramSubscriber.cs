using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Lykke.Service.TelegramReporter.Core.Services
{
    public interface ITelegramSubscriber
    {
        string Command { get; }

        Task ProcessMessageAsync(ITelegramSender telegramSender, Message message);

        Task ProcessCallbackQuery(ITelegramSender telegramSender, CallbackQuery callbackQuery);
    }
}
