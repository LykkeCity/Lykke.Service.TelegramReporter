﻿using Lykke.Service.TelegramReporter.Core.Services;
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
using Lykke.Service.TelegramReporter.Core.Services.Balance;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using Lykke.Service.TelegramReporter.Core.Services.SpreadEngine;
using Lykke.Service.TelegramReporter.Services.Balance;
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
        private readonly IBalanceWarningProvider _balanceWarningProvider;
        private readonly ITelegramSender _telegramSender;

        private bool _initialized;

        private readonly ConcurrentDictionary<long, ChatPublisher> _cmlPublishers = new ConcurrentDictionary<long, ChatPublisher>();
        private readonly ConcurrentDictionary<long, ChatPublisher> _sePublishers = new ConcurrentDictionary<long, ChatPublisher>();
        private readonly ConcurrentDictionary<long, ChatPublisher> _balancePublishers = new ConcurrentDictionary<long, ChatPublisher>();

        public ChatPublisherService(IChatPublisherSettingsRepository repo, ILog log,
            ITelegramSender telegramSender,
            ICmlSummaryProvider cmlSummaryProvider,
            ICmlStateProvider cmlStateProvider,
            ISpreadEngineStateProvider seStateProvider,
            IBalanceWarningProvider balanceWarningProvider)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = log.CreateComponentScope(nameof(ChatPublisherService));

            _cmlSummaryProvider = cmlSummaryProvider;
            _cmlStateProvider = cmlStateProvider;
            _seStateProvider = seStateProvider;
            _balanceWarningProvider = balanceWarningProvider;
            _telegramSender = telegramSender;
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetCmlChatPublishers()
        {
            return await _repo.GetCmlChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetSeChatPublishers()
        {
            return await _repo.GetSeChatPublisherSettings();
        }

        public async Task<IReadOnlyList<IChatPublisherSettings>> GetBalanceChatPublishers()
        {
            return await _repo.GetBalanceChatPublisherSettings();
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

        public async Task AddBalanceChatPublisherAsync(IChatPublisherSettings chatPublisher)
        {
            EnsureInitialized();
            await _repo.AddBalanceChatPublisherSettingsAsync(chatPublisher);
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

        public async Task RemoveBalanceChatPublisherAsync(string chatPublisherId)
        {
            EnsureInitialized();
            await _repo.RemoveBalanceChatPublisherSettingsAsync(chatPublisherId);
            await UpdateChatPublishers();
        }

        public void Start()
        {
            EnsureInitialized();
        }

        public void Stop()
        {
            foreach (var chatPublisher in _cmlPublishers.Values)
            {
                chatPublisher.Stop();
            }

            foreach (var chatPublisher in _sePublishers.Values)
            {
                chatPublisher.Stop();
            }

            foreach (var chatPublisher in _balancePublishers.Values)
            {
                chatPublisher.Stop();
            }
        }

        public void Dispose()
        {
            foreach (var chatPublisher in _cmlPublishers.Values)
            {
                chatPublisher.Dispose();
            }

            foreach (var chatPublisher in _sePublishers.Values)
            {
                chatPublisher.Dispose();
            }

            foreach (var chatPublisher in _balancePublishers.Values)
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
            var cmlPublisherSettings = await _repo.GetCmlChatPublisherSettings();
            var sePublisherSettings = await _repo.GetSeChatPublisherSettings();
            var balancePublisherSettings = await _repo.GetBalanceChatPublisherSettings();

            foreach (var publisherSettings in cmlPublisherSettings)
            {
                AddCmlPublisherIfNeeded(publisherSettings);                
            }
            
            CleanCmlPublishers(cmlPublisherSettings);

            foreach (var publisherSettings in sePublisherSettings)
            {
                AddSePublisherIfNeeded(publisherSettings);
            }

            CleanSePublishers(sePublisherSettings);

            foreach (var publisherSettings in balancePublisherSettings)
            {
                AddBalancePublisherIfNeeded(publisherSettings);
            }

            CleanBalancePublishers(balancePublisherSettings);
        }

        private void AddCmlPublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var exist = _cmlPublishers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                var newChatPublisher = new CmlPublisher(_telegramSender,
                    _cmlSummaryProvider, _cmlStateProvider, publisherSettings, _log);

                newChatPublisher.Start();
                _cmlPublishers[publisherSettings.ChatId] = newChatPublisher;
            }
        }

        private void AddSePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var exist = _sePublishers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                var newChatPublisher = new SpreadEnginePublisher(_telegramSender,
                    _seStateProvider, publisherSettings, _log);

                newChatPublisher.Start();
                _sePublishers[publisherSettings.ChatId] = newChatPublisher;
            }
        }

        private void AddBalancePublisherIfNeeded(IChatPublisherSettings publisherSettings)
        {
            var exist = _balancePublishers.ContainsKey(publisherSettings.ChatId);
            if (!exist)
            {
                var newChatPublisher = new BalancePublisher(_telegramSender,
                    _balanceWarningProvider, publisherSettings, _log);

                newChatPublisher.Start();
                _balancePublishers[publisherSettings.ChatId] = newChatPublisher;
            }
        }

        private void CleanCmlPublishers(IReadOnlyList<IChatPublisherSettings> cmlPublisherSettings)
        {
            foreach (var chatId in _cmlPublishers.Keys)
            {
                if (cmlPublisherSettings.All(x => x.ChatId != chatId))
                {
                    _cmlPublishers[chatId].Stop();
                    _cmlPublishers[chatId].Dispose();
                    _cmlPublishers.Remove(chatId, out _);
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
                    _sePublishers[chatId].Dispose();
                    _sePublishers.Remove(chatId, out _);
                }
            }
        }

        private void CleanBalancePublishers(IReadOnlyList<IChatPublisherSettings> balancePublisherSettings)
        {
            foreach (var chatId in _balancePublishers.Keys)
            {
                if (balancePublisherSettings.All(x => x.ChatId != chatId))
                {
                    _balancePublishers[chatId].Stop();
                    _balancePublishers[chatId].Dispose();
                    _balancePublishers.Remove(chatId, out _);
                }
            }
        }
    }
}
