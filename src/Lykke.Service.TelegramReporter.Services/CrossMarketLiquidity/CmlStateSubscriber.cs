using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Settings;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlStateSubscriber : ChatSubscriber
    {
        private const string CmlStateCommand = "/cmlstate";

        private static readonly Regex InstanceIdRegex =
            new Regex($"{CmlStateCommand}\\s+(?<instanceId>\\S+)", RegexOptions.Multiline);

        private readonly ICmlStateProvider _cmlStateProvider;
        private readonly ICrossMarketLiquidityInstanceManager _crossMarketLiquidityInstanceManager;

        public CmlStateSubscriber(ICmlStateProvider cmlStateProvider,
            ICrossMarketLiquidityInstanceManager crossMarketLiquidityInstanceManager,
            PublisherSettings publisherSettings)
            : base (publisherSettings)
        {
            _cmlStateProvider = cmlStateProvider;
            _crossMarketLiquidityInstanceManager = crossMarketLiquidityInstanceManager;
        }

        public override string Command => CmlStateCommand;

        public override async Task ProcessMessageInternalAsync(ITelegramSender telegramSender, Message message)
        {
            var keyboard =
                new InlineKeyboardMarkup(
                    _crossMarketLiquidityInstanceManager.Keys.Select(k =>
                        InlineKeyboardButton.WithCallbackData(k, $"{CmlStateCommand} {k}")));

            await telegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                "Select an instance",
                replyToMessageId: message.MessageId,
                replyMarkup: keyboard);
        }

        public override async Task ProcessCallbackQueryInternal(ITelegramSender telegramSender, CallbackQuery callbackQuery)
        {
            var instanceId = ExtractInstanceId(callbackQuery.Data);
            var result = await _cmlStateProvider.GetStateMessageAsync(instanceId);

            await telegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                result,
                replyToMessageId: callbackQuery.Message.MessageId);
        }        

        private string ExtractInstanceId(string data)
        {
            var match = InstanceIdRegex.Match(data);
            if (!match.Success)
                return null;

            var group = match.Groups["instanceId"];
            if (!group.Success)
                return null;

            return group.Value;
        }
    }
}
