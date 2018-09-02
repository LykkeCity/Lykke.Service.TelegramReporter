using Lykke.Service.TelegramReporter.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Balances.Client;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.Balance;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Services.Balance;
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
        private readonly ILog _log;
        private readonly ILogFactory _logFactory;

        private readonly INettingEngineStateProvider _neStateProvider;
        private readonly IBalanceWarningProvider _balanceWarningProvider;
        private readonly IExternalBalanceWarningProvider _externalBalanceWarningProvider;
        private readonly ITelegramSender _telegramSender;

        private bool _initialized;

        private readonly ConcurrentDictionary<long, ChatPublisher> _nePublishers = new ConcurrentDictionary<long, ChatPublisher>();
        private readonly ConcurrentDictionary<long, ChatPublisher> _balancePublishers = new ConcurrentDictionary<long, ChatPublisher>();
        private readonly ConcurrentDictionary<long, ChatPublisher> _externalBalancePublishers = new ConcurrentDictionary<long, ChatPublisher>();

        public ChatPublisherService(IChatPublisherSettingsRepository repo,
            IBalanceWarningRepository balanceWarningRepository,
            IExternalBalanceWarningRepository externalBalanceWarningRepository,
            IBalancesClient balancesClient,
            INettingEngineInstanceManager nettingEngineInstanceManager,
            ILogFactory logFactory,
            ITelegramSender telegramSender,
            INettingEngineStateProvider neStateProvider,
            IBalanceWarningProvider balanceWarningProvider,
            IExternalBalanceWarningProvider externalBalanceWarningProvider)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = logFactory.CreateLog(this);
            _logFactory = logFactory;

            _balanceWarningRepository = balanceWarningRepository;
            _externalBalanceWarningRepository = externalBalanceWarningRepository;
            _balancesClient = balancesClient;
            _nettingEngineInstanceManager = nettingEngineInstanceManager;
            _neStateProvider = neStateProvider;
            _balanceWarningProvider = balanceWarningProvider;
            _externalBalanceWarningProvider = externalBalanceWarningProvider;
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
            foreach (var chatPublisher in _nePublishers.Values)
            {
                chatPublisher.Stop();
            }

            foreach (var chatPublisher in _balancePublishers.Values)
            {
                chatPublisher.Stop();
            }

            foreach (var chatPublisher in _externalBalancePublishers.Values)
            {
                chatPublisher.Stop();
            }
        }

        public void Dispose()
        {
            foreach (var chatPublisher in _nePublishers.Values)
            {
                chatPublisher.Dispose();
            }

            foreach (var chatPublisher in _balancePublishers.Values)
            {
                chatPublisher.Dispose();
            }

            foreach (var chatPublisher in _externalBalancePublishers.Values)
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

            CleanPublishers(nePublisherSettings, _nePublishers);
            foreach (var publisherSettings in nePublisherSettings)
            {
                AddNePublisherIfNeeded(publisherSettings);
            }

            CleanPublishers(balancePublisherSettings, _balancePublishers);
            foreach (var publisherSettings in balancePublisherSettings)
            {
                AddBalancePublisherIfNeeded(publisherSettings);
            }

            CleanPublishers(externalBalancePublisherSettings, _externalBalancePublishers);
            foreach (var publisherSettings in externalBalancePublisherSettings)
            {
                AddExternalBalancePublisherIfNeeded(publisherSettings);
            }            
        }

        private void AddNePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new NettingEnginePublisher(_telegramSender,
                _neStateProvider, publisherSettings, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _nePublishers, newChatPublisher);
        }

        private void AddBalancePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new BalancePublisher(_telegramSender, _balanceWarningRepository, _balancesClient,
                _balanceWarningProvider, publisherSettings, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _nePublishers, newChatPublisher);
        }

        private void AddExternalBalancePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var newChatPublisher = new ExternalBalancePublisher(_telegramSender, _externalBalanceWarningRepository,
                _nettingEngineInstanceManager, _externalBalanceWarningProvider, publisherSettings, _logFactory);

            AddPublisherIfNeeded(publisherSettings, _externalBalancePublishers, newChatPublisher);
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
