﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;

namespace Lykke.Service.TelegramReporter.Services.LquidityEngineTrades
{
    public class LiquidityEngineTradesPublisher : ChatPublisher
    {
        private readonly LiquidityEngineUrlSettings _settings;

        private Dictionary<string, IPositionsApi> _clients = new Dictionary<string, IPositionsApi>();

        public LiquidityEngineTradesPublisher(ITelegramSender telegramSender, IChatPublisherSettings publisherSettings,
            LiquidityEngineUrlSettings settings,
            ILogFactory logFactory) 
            : base(telegramSender, publisherSettings, logFactory)
        {
            _settings = settings;
        }

        public override async void Publish()
        {
            if (!_clients.Any())
            {
                foreach (var url in _settings.Urls)
                {
                    var client = CreateApiClient(url);
                    _clients.Add(url, client);
                }
            }

            foreach (var positionsApi in _clients)
            {
                await CheckApi(positionsApi.Key, positionsApi.Value);
            }
        }
        
        private Dictionary<string, DateTime> _lastClose = new Dictionary<string, DateTime>();

        private async Task CheckApi(string key, IPositionsApi positionsApi)
        {
            var fromDate = DateTime.UtcNow.Date.AddDays(-1);
            var toDate = DateTime.UtcNow.Date.AddDays(+1);
            
            if (!_lastClose.TryGetValue(key, out var lastClose))
            {
                lastClose = DateTime.UtcNow;
                _lastClose[key] = lastClose;
            }

            var countTrade = 0;
            try
            {
                var data = await positionsApi.GetAllAsync(fromDate, toDate, 500);

                var positions = data.Where(e => e.CloseDate > lastClose).ToList();

                foreach (var model in positions.OrderBy(e => e.CloseDate))
                {
                    var message =
                        $"{model.AssetPairId}; PL={model.PnL}; Trade: {model.Type.ToString()}; Volume: {model.Volume}; OpenPrice: {model.Price}; ClosePrice: {model.ClosePrice}";

                    if (positions.Count >= 470) message += "; !!!max count of position in day, please add limit!!!";
                    await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId, message);

                    _lastClose[key] = model.CloseDate;
                    countTrade++;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                _lastClose[key] = DateTime.UtcNow;
            }

            Log.Info($"Check api complite. Found: {countTrade} trades. Api: {key}. LastTime: {lastClose:yyyy-MM-dd HH:mm:ss}");
        }

        private IPositionsApi CreateApiClient(string url)
        {
            var generator = Lykke.HttpClientGenerator.HttpClientGenerator.BuildForUrl(url)
                .WithAdditionalCallsWrapper(new Lykke.HttpClientGenerator.Infrastructure.ExceptionHandlerCallsWrapper())
                .WithoutRetries()
                .WithoutCaching()
                .Create();

            var client = generator.Generate<IPositionsApi>();

            return client;
        }
    }
}
