using System;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.MarketMakerReports.Client;
using Lykke.Service.MarketMakerReports.Client.Models.InventorySnapshots;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEnginePublisher : ChatPublisher
    {
        private readonly INettingEngineStateProvider _nettingEngineStateProvider;
        private readonly IMarketMakerReportsClient _marketMakerReportsClient;

        private bool _initialized;

        public NettingEnginePublisher(ITelegramSender telegramSender,
            INettingEngineStateProvider nettingEngineStateProvider,
            IMarketMakerReportsClient marketMakerReportsClient,
            IChatPublisherSettings publisherSettings, ILogFactory logFactory)
            : base(telegramSender, publisherSettings, logFactory)
        {
            _nettingEngineStateProvider = nettingEngineStateProvider;
            _marketMakerReportsClient = marketMakerReportsClient;

            EnsureInitialized();
        }

        public InventorySnapshotModel PrevSnapshot { get; private set; }

        public Task<InventorySnapshotModel> GetCurrentInventorySnapshot()
        {
            return _marketMakerReportsClient.InventorySnapshotsApi.GetLastAsync();
        }

        public override async void Publish()
        {
            EnsureInitialized();

            try
            {
                var snapshot = await GetCurrentInventorySnapshot();

                await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                    await _nettingEngineStateProvider.GetStateMessageAsync(PrevSnapshot, snapshot));

                PrevSnapshot = snapshot;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;

            var task = Task.Run(Initialize);
            Task.WaitAll(task);

            _initialized = true;
        }

        private async Task Initialize()
        {
            try
            {
                PrevSnapshot = await _marketMakerReportsClient.InventorySnapshotsApi.GetLastAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
