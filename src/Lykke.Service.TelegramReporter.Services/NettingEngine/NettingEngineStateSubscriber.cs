using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.MarketMakerReports.Client;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Telegram.Bot.Types;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineStateSubscriber : ChatSubscriber
    {
        private const string NettingEngineStateCommand = "/nettingenginestate";

        private readonly INettingEngineStateProvider _nettingEngineStateProvider;
        private readonly IMarketMakerReportsClient _marketMakerReportsClient;
        private readonly IChatPublisherStateService _chatPublisherStateService;

        public NettingEngineStateSubscriber(INettingEngineStateProvider nettingEngineStateProvider,
            IMarketMakerReportsClient marketMakerReportsClient, IChatPublisherStateService chatPublisherStateService,
            IChatPublisherSettingsRepository repo, ILogFactory logFactory)
            : base(repo, logFactory)
        {
            _nettingEngineStateProvider = nettingEngineStateProvider;
            _marketMakerReportsClient = marketMakerReportsClient;
            _chatPublisherStateService = chatPublisherStateService;
        }

        public override string Command => NettingEngineStateCommand;

        public override async Task ProcessMessageInternalAsync(ITelegramSender telegramSender, Message message)
        {
            try
            {
                var allowedChatIds = await GetAllowedChatIds();
                if (allowedChatIds.Contains(message.Chat.Id))
                {
                    var chatPublisher = (NettingEnginePublisher)_chatPublisherStateService.NePublishers.Single(x => x.Key == message.Chat.Id).Value;

                    var snapshot = await _marketMakerReportsClient.InventorySnapshotsApi.GetLastAsync();

                    await telegramSender.SendTextMessageAsync(message.Chat.Id,
                        await _nettingEngineStateProvider.GetStateMessageAsync(chatPublisher.PrevSnapshot, snapshot),
                        replyToMessageId: message.MessageId);
                }
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(NettingEngineStateSubscriber), nameof(ProcessMessageInternalAsync), "", ex);
            }
        }

        public override Task ProcessCallbackQueryInternal(ITelegramSender telegramSender,
            CallbackQuery callbackQuery)
        {
            return Task.CompletedTask;
        }

        protected override async Task<long[]> GetAllowedChatIds()
        {
            return (await Repo.GetNeChatPublisherSettings()).Select(x => x.ChatId).ToArray();
        }
    }
}
