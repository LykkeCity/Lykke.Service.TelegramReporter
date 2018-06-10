using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlStateSubscriber : ITelegramSubscriber
    {
        private const string CmlStateCommand = "/cmlstate";

        private readonly ICmlStateProvider _cmlStateProvider;
        private ICrossMarketLiquidityInstanceManager _crossMarketLiquidityInstanceManager;

        public CmlStateSubscriber(ICmlStateProvider cmlStateProvider,
            ICrossMarketLiquidityInstanceManager crossMarketLiquidityInstanceManager)
        {
            _cmlStateProvider = cmlStateProvider;
            _crossMarketLiquidityInstanceManager = crossMarketLiquidityInstanceManager;
        }

        public string Command => CmlStateCommand;

        public async Task ProcessMessageAsync(ITelegramSender telegramSender, Message message)
        {
            var keyboard =
                new InlineKeyboardMarkup(
                    _crossMarketLiquidityInstanceManager.Keys.Select(k =>
                        InlineKeyboardButton.WithCallbackData(k, $"{CmlStateCommand} {k}")));

            await telegramSender.SendTextMessageAsync(message.Chat.Id,
                "Select an instance",
                replyToMessageId: message.MessageId,
                replyMarkup: keyboard);
        }

        public async Task ProcessCallbackQuery(ITelegramSender telegramSender,
            CallbackQuery callbackQuery)
        {
            string instanceId = ExtractInstanceId(callbackQuery.Data);
            string result = await _cmlStateProvider.GetStateMessageAsync(instanceId);

            await telegramSender.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                result,
                replyToMessageId: callbackQuery.Message.MessageId);
        }

        private static readonly Regex _instanceIdRegex =
            new Regex($"{CmlStateCommand}\\s+(?<instanceId>\\S+)", RegexOptions.Multiline);

        private string ExtractInstanceId(string data)
        {
            Match match = _instanceIdRegex.Match(data);
            if (!match.Success)
                return null;

            Group group = match.Groups["instanceId"];
            if (!group.Success)
                return null;

            return group.Value;
        }
    }
}
