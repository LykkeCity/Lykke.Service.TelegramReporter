using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineStateSubscriber : ChatSubscriber
    {
        private const string NettingEngineStateCommand = "/nettingenginestate";

        private static readonly Regex InstanceIdRegex =
            new Regex($"{NettingEngineStateCommand}\\s+(?<instanceId>\\S+)", RegexOptions.Multiline);

        private readonly INettingEngineStateProvider _nettingEngineStateProvider;
        private readonly INettingEngineInstanceManager _nettingEngineInstanceManager;

        public NettingEngineStateSubscriber(INettingEngineStateProvider nettingEngineStateProvider,
            INettingEngineInstanceManager nettingEngineInstanceManager,
            IChatPublisherSettingsRepository repo, ILogFactory logFactory)
            : base(repo, logFactory)
        {
            _nettingEngineStateProvider = nettingEngineStateProvider;
            _nettingEngineInstanceManager = nettingEngineInstanceManager;
        }

        public override string Command => NettingEngineStateCommand;

        public override async Task ProcessMessageInternalAsync(ITelegramSender telegramSender, Message message)
        {
            try
            {
                var allowedChatIds = await GetAllowedChatIds();
                if (allowedChatIds.Contains(message.Chat.Id))
                {
                    var keyboard =
                        new InlineKeyboardMarkup(
                            _nettingEngineInstanceManager.Instances.Select(k =>
                                InlineKeyboardButton.WithCallbackData(k.DisplayName,
                                    $"{NettingEngineStateCommand} {k.Index}")));

                    await telegramSender.SendTextMessageAsync(message.Chat.Id,
                        "Select an instance",
                        replyToMessageId: message.MessageId,
                        replyMarkup: keyboard);
                }
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(NettingEngineStateSubscriber), nameof(ProcessMessageInternalAsync), "", ex);
            }
        }

        public override async Task ProcessCallbackQueryInternal(ITelegramSender telegramSender,
            CallbackQuery callbackQuery)
        {
            try
            {
                var allowedChatIds = await GetAllowedChatIds();
                if (allowedChatIds.Contains(callbackQuery.Message.Chat.Id))
                {
                    var instanceId = ExtractInstanceId(callbackQuery.Data);
                    var result = await _nettingEngineStateProvider.GetStateMessageAsync(int.Parse(instanceId));

                    await telegramSender.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                        result,
                        replyToMessageId: callbackQuery.Message.MessageId);
                }
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(NettingEngineStateSubscriber), nameof(ProcessCallbackQueryInternal), "", ex);
            }
        }

        protected override async Task<long[]> GetAllowedChatIds()
        {
            return (await Repo.GetSeChatPublisherSettings()).Select(x => x.ChatId).ToArray();
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
