using Lykke.Service.TelegramReporter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Balances.Client;
using Lykke.Service.MarketMakerArbitrageDetector.Client;
using Lykke.Service.MarketMakerReports.Client;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.LiquidityEngine;
using Lykke.Service.TelegramReporter.Core.Services.MarketMakerArbitrages;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Services.LiquidityEngine;
using Lykke.Service.TelegramReporter.Services.MarketMakerArbitrages;
using Lykke.Service.TelegramReporter.Services.NettingEngine;

namespace Lykke.Service.TelegramReporter.Services
{
    public class ChatPublisherService : IChatPublisherService, IStartable, IStopable
    {
        private readonly IChatPublisherSettingsRepository _repo;
        private readonly IBalanceWarningRepository _balanceWarningRepository;
        private readonly IExternalBalanceWarningRepository _externalBalanceWarningRepository;
        private readonly IBalancesClient _balancesClient;
        private readonly INettingEngineInstanceManager _nettingEngineInstanceManager;
        private readonly IChatPublisherStateService _chatPublisherStateService;
        private readonly ILog _log;
        private readonly ILogFactory _logFactory;

        private readonly INettingEngineStateProvider _neStateProvider;
        private readonly IMarketMakerReportsClient _marketMakerReportsClient;
        private readonly IMarketMakerArbitrageDetectorClient _marketMakerArbitrageDetectorClient;
        private readonly IMarketMakerArbitragesWarningProvider _marketMakerArbitragesWarningProvider;
        private readonly IBalanceWarningProvider _balanceWarningProvider;
        private readonly IExternalBalanceWarningProvider _externalBalanceWarningProvider;
        private readonly LiquidityEngineUrlSettings _liquidityEngineUrlSettings;
        private readonly ITelegramSender _telegramSender;

        private bool _initialized;

        public ChatPublisherService(IChatPublisherSettingsRepository repo,
            IBalanceWarningRepository balanceWarningRepository,
            IExternalBalanceWarningRepository externalBalanceWarningRepository,
            IBalancesClient balancesClient,
            INettingEngineInstanceManager nettingEngineInstanceManager,
            IMarketMakerReportsClient marketMakerReportsClient,
            IMarketMakerArbitrageDetectorClient marketMakerArbitrageDetectorClient,
            IMarketMakerArbitragesWarningProvider marketMakerArbitragesWarningProvider,
            IChatPublisherStateService chatPublisherStateService,
            ILogFactory logFactory,
            ITelegramSender telegramSender,
            INettingEngineStateProvider neStateProvider,
            IBalanceWarningProvider balanceWarningProvider,
            IExternalBalanceWarningProvider externalBalanceWarningProvider,
            LiquidityEngineUrlSettings liquidityEngineUrlSettings)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = logFactory.CreateLog(this);
            _logFactory = logFactory;

            _chatPublisherStateService = chatPublisherStateService;
            _balanceWarningRepository = balanceWarningRepository;
            _externalBalanceWarningRepository = externalBalanceWarningRepository;
            _balancesClient = balancesClient;
            _nettingEngineInstanceManager = nettingEngineInstanceManager;
            _marketMakerReportsClient = marketMakerReportsClient;
            _marketMakerArbitrageDetectorClient = marketMakerArbitrageDetectorClient;
            _marketMakerArbitragesWarningProvider = marketMakerArbitragesWarningProvider;
            _neStateProvider = neStateProvider;
            _balanceWarningProvider = balanceWarningProvider;
            _externalBalanceWarningProvider = externalBalanceWarningProvider;
            _liquidityEngineUrlSettings = liquidityEngineUrlSettings;
            _telegramSender = telegramSender;
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetNeChatPublishersAsync()
        {
            EnsureInitialized();
            return await _repo.GetNeChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetBalanceChatPublishersAsync()
        {
            EnsureInitialized();
            return await _repo.GetBalanceChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetExternalBalanceChatPublishersAsync()
        {
            EnsureInitialized();
            return await _repo.GetExternalBalanceChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetWalletsRebalancerChatPublishersAsync()
        {
            EnsureInitialized();
            return await _repo.GetWalletsRebalancerChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetMarketMakerArbitragesChatPublishersAsync()
        {
            EnsureInitialized();
            return await _repo.GetMarketMakerArbitragesChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetLiquidityEngineTradesChatPublishersAsync()
        {
            EnsureInitialized();
            return await _repo.GetLiquidityEngineTradesChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetLiquidityEngineSummaryChatPublishersAsync()
        {
            EnsureInitialized();
            return await _repo.GetLiquidityEngineSummaryChatPublisherSettings();
        }

        public async Task AddNeChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddNeChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task AddBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddBalanceChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task AddExternalBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddExternalBalanceChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task AddWalletsRebalancerChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddWalletsRebalancerChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task AddMarketMakerArbitragesChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddMarketMakerArbitragesChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task AddLiquidityEngineTradesChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddLiquidityEngineTradesChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task AddLiquidityEngineSummaryChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddLiquidityEngineSummaryChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task RemoveNeChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveNeChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public async Task RemoveBalanceChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveBalanceChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public async Task RemoveExternalBalanceChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveExternalBalanceChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public async Task RemoveWalletsRebalancerChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveWalletsRebalancerChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public async Task RemoveMarketMakerArbitragesChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveMarketMakerArbitragesChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public async Task RemoveLiquidityEngineTradesChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveLiquidityEngineTradesChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public async Task RemoveLiquidityEngineSummaryChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveLiquidityEngineSummaryChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public async Task<IReadOnlyList<IBalanceWarning>> GetBalancesWarningsAsync()
        {
            return await _balanceWarningRepository.GetBalancesWarnings();
        }

        public async Task<IReadOnlyList<IExternalBalanceWarning>> GetExternalBalancesWarningsAsync()
        {
            return await _externalBalanceWarningRepository.GetExternalBalancesWarnings();
        }

        public async Task AddBalanceWarningAsync(IBalanceWarning balanceWarning)
        {
            await _balanceWarningRepository.AddBalanceWarningAsync(balanceWarning);
        }

        public async Task AddExternalBalanceWarningAsync(IExternalBalanceWarning balanceWarning)
        {
            await _externalBalanceWarningRepository.AddExternalBalanceWarningAsync(balanceWarning);
        }

        public async Task RemoveBalanceWarningAsync(string clientId, string assetId)
        {
            await _balanceWarningRepository.RemoveBalanceWarningAsync(clientId, assetId);
        }

        public async Task RemoveExternalBalanceWarningAsync(string exchange, string assetId)
        {
            await _externalBalanceWarningRepository.RemoveExternalBalanceWarningAsync(exchange, assetId);
        }

        public void Start()
        {
            EnsureInitialized();
        }

        public void Stop()
        {
            StopPublishers(_chatPublisherStateService.NePublishers);
            StopPublishers(_chatPublisherStateService.BalancePublishers);
            StopPublishers(_chatPublisherStateService.ExternalBalancePublishers);
            StopPublishers(_chatPublisherStateService.MarketMakerArbitragesPublishers);
        }

        private static void StopPublishers(IDictionary<long, ChatPublisher> publishers)
        {
            foreach (var chatPublisher in publishers.Values)
            {
                chatPublisher.Stop();
            }
        }

        public void Dispose()
        {
            DisposePublishers(_chatPublisherStateService.NePublishers);
            DisposePublishers(_chatPublisherStateService.BalancePublishers);
            DisposePublishers(_chatPublisherStateService.ExternalBalancePublishers);
            DisposePublishers(_chatPublisherStateService.MarketMakerArbitragesPublishers);
        }

        private static void DisposePublishers(IDictionary<long, ChatPublisher> publishers)
        {
            foreach (var chatPublisher in publishers.Values)
            {
                chatPublisher.Dispose();
            }
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;

            var task = Task.Run(UpdateChatPublishers);
            Task.WaitAll(task);

            _initialized = true;
        }

        private async Task UpdateChatPublishers()
        {
            var nePublisherSettings = await _repo.GetNeChatPublisherSettings();
            var balancePublisherSettings = await _repo.GetBalanceChatPublisherSettings();
            var externalBalancePublisherSettings = await _repo.GetExternalBalanceChatPublisherSettings();
            var marketMakerArbitragesPublisherSettings = await _repo.GetMarketMakerArbitragesChatPublisherSettings();
            var liquidityEngineTradesPublisherSettings = await _repo.GetLiquidityEngineTradesChatPublisherSettings();
            var liquidityEngineSummaryPublisherSettings = await _repo.GetLiquidityEngineSummaryChatPublisherSettings();

            CleanPublishers(nePublisherSettings, _chatPublisherStateService.NePublishers);
            foreach (var publisherSettings in nePublisherSettings)
            {
                AddNePublisherIfNeeded(publisherSettings);
            }

            CleanPublishers(balancePublisherSettings, _chatPublisherStateService.BalancePublishers);
            foreach (var publisherSettings in balancePublisherSettings)
            {
                AddBalancePublisherIfNeeded(publisherSettings);
            }

            CleanPublishers(externalBalancePublisherSettings, _chatPublisherStateService.ExternalBalancePublishers);
            foreach (var publisherSettings in externalBalancePublisherSettings)
            {
                AddExternalBalancePublisherIfNeeded(publisherSettings);
            }

            CleanPublishers(marketMakerArbitragesPublisherSettings, _chatPublisherStateService.MarketMakerArbitragesPublishers);
            foreach (var publisherSettings in marketMakerArbitragesPublisherSettings)
            {
                AddMarketMakerArbitragesPublisherIfNeeded(publisherSettings);
            }

            CleanPublishers(liquidityEngineTradesPublisherSettings, _chatPublisherStateService.LiquidityEngineTradesPublishers);
            foreach (var publisherSettings in liquidityEngineTradesPublisherSettings)
            {
                AddLiquidityEngineTradesPublisherIfNeeded(publisherSettings);
            }

            CleanPublishers(liquidityEngineSummaryPublisherSettings, _chatPublisherStateService.LiquidityEngineSummaryPublishers);
            foreach (var publisherSettings in liquidityEngineSummaryPublisherSettings)
            {
                AddLiquidityEngineSummaryPublisherIfNeeded(publisherSettings);
            }
        }

        private void AddNePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new NettingEnginePublisher(_telegramSender,
                _neStateProvider, _marketMakerReportsClient, publisherSettings, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _chatPublisherStateService.NePublishers, newChatPublisher);
        }

        private void AddBalancePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new BalancePublisher(_telegramSender, _balanceWarningRepository, _balancesClient,
                _balanceWarningProvider, publisherSettings, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _chatPublisherStateService.BalancePublishers, newChatPublisher);
        }

        private void AddExternalBalancePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new ExternalBalancePublisher(_telegramSender, _externalBalanceWarningRepository,
                _nettingEngineInstanceManager, _externalBalanceWarningProvider, publisherSettings, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _chatPublisherStateService.ExternalBalancePublishers, newChatPublisher);
        }

        private void AddMarketMakerArbitragesPublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new MarketMakerArbitragesPublisher(_telegramSender, publisherSettings,
                _marketMakerArbitragesWarningProvider, _marketMakerArbitrageDetectorClient, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _chatPublisherStateService.MarketMakerArbitragesPublishers, newChatPublisher);
        }

        private void AddLiquidityEngineTradesPublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new LiquidityEngineTradesPublisher(_telegramSender, publisherSettings, _liquidityEngineUrlSettings, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _chatPublisherStateService.LiquidityEngineTradesPublishers, newChatPublisher);
        }

        private void AddLiquidityEngineSummaryPublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new LiquidityEngineTradesPublisher(_telegramSender, publisherSettings, _liquidityEngineUrlSettings, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _chatPublisherStateService.LiquidityEngineSummaryPublishers, newChatPublisher);
        }

        private static void AddPublisherIfNeeded(IChatPublisherSettings publisherSettings, IDictionary<long, ChatPublisher> publishers, ChatPublisher newChatPublisher)
        {
            var exist = publishers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                newChatPublisher.Start();
                publishers[publisherSettings.ChatId] = newChatPublisher;
            }
            else
            {
                if (publisherSettings.TimeSpan < publishers[publisherSettings.ChatId].PublisherSettings.TimeSpan)
                {
                    publishers[publisherSettings.ChatId].Stop();
                    publishers[publisherSettings.ChatId].Dispose();
                    publishers.Remove(publisherSettings.ChatId, out _);

                    newChatPublisher.Start();
                    publishers[publisherSettings.ChatId] = newChatPublisher;
                }
            }
        }

        private void CleanPublishers(IReadOnlyList<IChatPublisherSettings> publisherSettings, IDictionary<long, ChatPublisher> publishers)
        {
            foreach (var publisher in publishers)
            {
                var chatId = publisher.Key;
                if (!publisherSettings.Any(x => x.ChatId == chatId && x.TimeSpan == publisher.Value.PublisherSettings.TimeSpan))
                {
                    publishers[chatId].Stop();
                    publishers[chatId].Dispose();
                    publishers.Remove(chatId, out _);
                }
            }
        }
    }
}
