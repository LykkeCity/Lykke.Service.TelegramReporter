using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;

namespace Lykke.Service.TelegramReporter.Services.LquidityEngineTrades
{
    public class LiquidityEngineTradesPublisher : ChatPublisher
    {
        private readonly LiquidityEngineUrlSettings _settings;

        private Dictionary<string, IReportsApi> _clients = new Dictionary<string, IReportsApi>();

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

            foreach (var reportsApi in _clients)
            {
                await CheckApi(reportsApi.Key, reportsApi.Value);
            }
        }
        
        private Dictionary<string, DateTime> _lastClose = new Dictionary<string, DateTime>();

        private async Task CheckApi(string key, IReportsApi reportsApi)
        {
            var fromDate = DateTime.UtcNow.Date.AddDays(-1);
            var toDate = DateTime.UtcNow.Date.AddDays(+1);
            
            if (!_lastClose.TryGetValue(key, out var lastClose))
            {
                lastClose = DateTime.UtcNow;
                _lastClose[key] = lastClose;
            }

            Log.Info($"Check api Start. trades. Api: {key}. LastTime: {lastClose:yyyy-MM-dd HH:mm:ss}");

            var countTrade = 0;
            try
            {
                var data = await reportsApi.GetPositionsReportAsync(fromDate, toDate, 1500);

                var positions = data.Where(e => e.CloseDate > lastClose).ToList();

                foreach (var model in positions.OrderBy(e => e.CloseDate))
                {
                    var pnL = model.PnL ?? 0;
                    var closePrice = model.ClosePrice ?? 0;
                    var pnLStr = model.PnLUsd.HasValue ? $"{Math.Round(model.PnLUsd.Value, 4)}$" : $"{Math.Round(pnL, 4)} [quote asset]";

                    var message =
                        $"{model.AssetPairId}; " +
                        $"PL={pnLStr}; " +
                        $"{(model.Type == PositionType.Short ? "Sell" : "Buy")}; " +
                        $"Volume: {Math.Round(model.Volume, 6)}; " +
                        $"OpenPrice: {Math.Round(model.Price, 6)}; " +
                        $"ClosePrice: {Math.Round(closePrice, 6)}; " +
                        $"Close: {model.CloseDate:MM-dd HH:mm:ss}";

                    if (positions.Count >= 470) message += "; !!!max count of position in day, please add limit!!!";
                    await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId, message);

                    if (model.CloseDate.HasValue)
                        _lastClose[key] = model.CloseDate.Value;

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

        private IReportsApi CreateApiClient(string url)
        {
            var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(url)
                .WithAdditionalCallsWrapper(new HttpClientGenerator.Infrastructure.ExceptionHandlerCallsWrapper())
                .WithoutRetries()
                .WithoutCaching()
                .Create();

            var client = generator.Generate<IReportsApi>();

            return client;
        }
    }
}
