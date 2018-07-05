using Lykke.Service.TelegramReporter.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Service.TelegramReporter.Core.Domain;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using Lykke.Service.TelegramReporter.Core.Services.SpreadEngine;
using Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity;
using Lykke.Service.TelegramReporter.Services.SpreadEngine;

namespace Lykke.Service.TelegramReporter.Services
{
    public class ChatPublisherService : IChatPublisherService, IStartable, IStopable
    {
        private readonly IChatPublisherSettingsRepository _repo;
        private readonly ILog _log;

        private readonly ICmlSummaryProvider _cmlSummaryProvider;
        private readonly ICmlStateProvider _cmlStateProvider;
        private readonly ISpreadEngineStateProvider _seStateProvider;
        private readonly ICrossMarketLiquidityInstanceManager _crossMarketLiquidityInstanceManager;
        private readonly ISpreadEngineInstanceManager _spreadEngineInstanceManager;
        private readonly ITelegramSender _telegramSender;

        private bool _initialized;

        private readonly ConcurrentDictionary<long, ChatPublisher> _cmlPublishers = new ConcurrentDictionary<long, ChatPublisher>();
        private readonly ConcurrentDictionary<long, ChatPublisher> _sePublishers = new ConcurrentDictionary<long, ChatPublisher>();

        private readonly ConcurrentDictionary<long, ChatSubscriber> _cmlSummarySubscribers = new ConcurrentDictionary<long, ChatSubscriber>();
        private readonly ConcurrentDictionary<long, ChatSubscriber> _cmlStateSubscribers = new ConcurrentDictionary<long, ChatSubscriber>();
        private readonly ConcurrentDictionary<long, ChatSubscriber> _seStateSubscribers = new ConcurrentDictionary<long, ChatSubscriber>();

        public ChatPublisherService(IChatPublisherSettingsRepository repo, ILog log,
            ITelegramSender telegramSender,
            ICmlSummaryProvider cmlSummaryProvider,
            ICmlStateProvider cmlStateProvider,
            ISpreadEngineStateProvider seStateProvider,
            ICrossMarketLiquidityInstanceManager crossMarketLiquidityInstanceManager,
            ISpreadEngineInstanceManager spreadEngineInstanceManager)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = log.CreateComponentScope(nameof(ChatPublisherService));

            _cmlSummaryProvider = cmlSummaryProvider;
            _cmlStateProvider = cmlStateProvider;
            _seStateProvider = seStateProvider;
            _telegramSender = telegramSender;
            _crossMarketLiquidityInstanceManager = crossMarketLiquidityInstanceManager;
            _spreadEngineInstanceManager = spreadEngineInstanceManager;
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetCmlChatPublishers()
        {
            return await _repo.GetCmlChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetSeChatPublishers()
        {
            return await _repo.GetSeChatPublisherSettings();
        }

        public async Task AddCmlChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddCmlChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task AddSeChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddSeChatPublisherSettingsAsync(chatPublisher);
            await UpdateChatPublishers();
        }

        public async Task RemoveCmlChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveCmlChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public async Task RemoveSeChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveSeChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public void Start()
        {
            EnsureInitialized();
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
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
            var cmlPublisherSettings = await _repo.GetCmlChatPublisherSettings();
            var sePublisherSettings = await _repo.GetSeChatPublisherSettings();

            foreach (var publisherSettings in cmlPublisherSettings)
            {
                AddCmlSummarySubscriberIfNeeded(publisherSettings);                
                AddCmlStateSubscriberIfNeeded(publisherSettings);                
                AddCmlPublisherIfNeeded(publisherSettings);                
            }
            
            CleanCmlPublishers(cmlPublisherSettings);
            CleanCmlSummarySubscribers(cmlPublisherSettings);
            CleanCmlStateSubscribers(cmlPublisherSettings);

            foreach (var publisherSettings in sePublisherSettings)
            {
                AddSeStateSubscriberIfNeeded(publisherSettings);
                AddSePublisherIfNeeded(publisherSettings);
            }

            CleanSePublishers(sePublisherSettings);
            CleanSeStateSubscribers(sePublisherSettings);
        }

        private void AddCmlSummarySubscriberIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var exist = _cmlSummarySubscribers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                var newChatSubscriber = new CmlSummarySubscriber(
                    _cmlSummaryProvider, publisherSettings);

                _cmlSummarySubscribers[publisherSettings.ChatId] = newChatSubscriber;
            }
        }

        private void AddCmlStateSubscriberIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var exist = _cmlStateSubscribers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                var newChatSubscriber = new CmlStateSubscriber(
                    _cmlStateProvider, _crossMarketLiquidityInstanceManager, publisherSettings);

                _cmlStateSubscribers[publisherSettings.ChatId] = newChatSubscriber;
            }
        }

        private void AddCmlPublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var exist = _cmlPublishers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                var newChatPublisher = new CmlPublisher(_telegramSender,
                    _cmlSummaryProvider, _cmlStateProvider, publisherSettings);

                newChatPublisher.Start();
                _cmlPublishers[publisherSettings.ChatId] = newChatPublisher;
            }
        }

        private void AddSeStateSubscriberIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var exist = _seStateSubscribers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                var newChatSubscriber = new SpreadEngineStateSubscriber(
                    _seStateProvider, _spreadEngineInstanceManager, publisherSettings);

                _cmlStateSubscribers[publisherSettings.ChatId] = newChatSubscriber;
            }
        }

        private void AddSePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var exist = _sePublishers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                var newChatPublisher = new SpreadEnginePublisher(_telegramSender,
                    _seStateProvider, publisherSettings);

                newChatPublisher.Start();
                _sePublishers[publisherSettings.ChatId] = newChatPublisher;
            }
        }

        private void CleanCmlPublishers(IReadOnlyList<IChatPublisherSettings> cmlPublisherSettings)
        {
            foreach (var chatId in _cmlPublishers.Keys)
            {
                if (cmlPublisherSettings.All(x => x.ChatId != chatId))
                {
                    _cmlPublishers[chatId].Stop();
                    _cmlPublishers.Remove(chatId, out _);
                }
            }
        }

        private void CleanCmlSummarySubscribers(IReadOnlyList<IChatPublisherSettings> cmlPublisherSettings)
        {
            foreach (var chatId in _cmlSummarySubscribers.Keys)
            {
                if (cmlPublisherSettings.All(x => x.ChatId != chatId))
                {
                    _cmlSummarySubscribers.Remove(chatId, out _);
                }
            }
        }

        private void CleanCmlStateSubscribers(IReadOnlyList<IChatPublisherSettings> cmlPublisherSettings)
        {
            foreach (var chatId in _cmlStateSubscribers.Keys)
            {
                if (cmlPublisherSettings.All(x => x.ChatId != chatId))
                {
                    _cmlStateSubscribers.Remove(chatId, out _);
                }
            }
        }

        private void CleanSePublishers(IReadOnlyList<IChatPublisherSettings> sePublisherSettings)
        {
            foreach (var chatId in _sePublishers.Keys)
            {
                if (sePublisherSettings.All(x => x.ChatId != chatId))
                {
                    _sePublishers[chatId].Stop();
                    _sePublishers.Remove(chatId, out _);
                }
            }
        }

        private void CleanSeStateSubscribers(IReadOnlyList<IChatPublisherSettings> sePublisherSettings)
        {
            foreach (var chatId in _seStateSubscribers.Keys)
            {
                if (sePublisherSettings.All(x => x.ChatId != chatId))
                {
                    _seStateSubscribers.Remove(chatId, out _);
                }
            }
        }
    }
}
