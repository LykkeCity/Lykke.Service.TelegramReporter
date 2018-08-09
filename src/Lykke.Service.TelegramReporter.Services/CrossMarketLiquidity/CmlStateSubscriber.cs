using System;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain;
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
            IChatPublisherSettingsRepository repo, ILogFactory logFactory)
            : base (repo, logFactory)
        {
            _cmlStateProvider = cmlStateProvider;
            _crossMarketLiquidityInstanceManager = crossMarketLiquidityInstanceManager;
        }

        public override string Command => CmlStateCommand;

        public override async Task ProcessMessageInternalAsync(ITelegramSender telegramSender, Message message)
        {
            try
            {
                var allowedChatIds = await GetAllowedChatIds();
                if (allowedChatIds.Contains(message.Chat.Id))
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
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(CmlStateSubscriber), nameof(ProcessMessageInternalAsync), "", ex);
            }
        }

        public override async Task ProcessCallbackQueryInternal(ITelegramSender telegramSender, CallbackQuery callbackQuery)
        {
            try
            {            
                var allowedChatIds = await GetAllowedChatIds();
                if (allowedChatIds.Contains(callbackQuery.Message.Chat.Id))
                {
                    var instanceId = ExtractInstanceId(callbackQuery.Data);
                    var result = await _cmlStateProvider.GetStateMessageAsync(instanceId);

                    await telegramSender.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                        result,
                        replyToMessageId: callbackQuery.Message.MessageId);
                }
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(CmlStateSubscriber), nameof(ProcessCallbackQueryInternal), "", ex);
            }
        }

        protected override async Task<long[]> GetAllowedChatIds()
        {
            return (await Repo.GetCmlChatPublisherSettings()).Select(x => x.ChatId).ToArray();
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
