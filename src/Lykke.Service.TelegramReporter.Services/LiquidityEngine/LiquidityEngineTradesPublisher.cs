using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.LiquidityEngine.Client;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;

namespace Lykke.Service.TelegramReporter.Services.LiquidityEngine
{
    public class LiquidityEngineTradesPublisher : ChatPublisher
    {
        private readonly LiquidityEngineUrlSettings _settings;
        private readonly Dictionary<string, ILiquidityEngineClient> _clients = new Dictionary<string, ILiquidityEngineClient>();
        private readonly Dictionary<string, DateTime> _lastClose = new Dictionary<string, DateTime>();
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;

        public LiquidityEngineTradesPublisher(ITelegramSender telegramSender, IChatPublisherSettings publisherSettings,
            LiquidityEngineUrlSettings settings, IAssetsServiceWithCache assetsServiceWithCache,
            ILogFactory logFactory)
            : base(telegramSender, publisherSettings, logFactory)
        {
            _settings = settings;
            _assetsServiceWithCache = assetsServiceWithCache;
        }

        public override async Task Publish()
        {
            if (!_clients.Any())
            {
                foreach (var url in _settings.Urls)
                {
                    var client = CreateClient(url);
                    _clients.Add(url, client);
                }
            }

            foreach (var key in _clients.Keys)
            {
                await CheckApi(key, _clients[key]);
            }
        }

        private async Task CheckApi(string key, ILiquidityEngineClient client)
        {
            var fromDate = DateTime.UtcNow.Date.AddDays(-1);
            var toDate = DateTime.UtcNow.Date.AddDays(+1);
            
            if (!_lastClose.TryGetValue(key, out var lastClose))
            {
                lastClose = DateTime.UtcNow;
                _lastClose[key] = lastClose;
            }

            Log.Info($"Check api started. Trades. Api: {key}. LastTime: {lastClose:yyyy-MM-dd HH:mm:ss}");

            var countTrade = 0;
            try
            {
                var data = await client.Reports.GetPositionsReportAsync(fromDate, toDate, 1500);
                var markups = await client.InstrumentMarkupsApi.GetAllAsync();

                var positions = data.Where(e => e.CloseDate > lastClose).ToList();

                foreach (var position in positions.OrderBy(e => e.CloseDate))
                {
                    var assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(position.AssetPairId);
                    var quoteAssetId = assetPair?.QuotingAssetId;
                    var asset = await _assetsServiceWithCache.TryGetAssetAsync(quoteAssetId);
                    var quoteAssetDisplayId = quoteAssetId == null ? null : asset.Id;
                    var quoteAssetStr = string.IsNullOrWhiteSpace(quoteAssetDisplayId) ? "[quote asset]" : quoteAssetDisplayId;
                    var markup = markups.Single(x => x.AssetPairId == position.AssetPairId);

                    var pnL = position.PnL ?? 0;
                    var closePrice = position.ClosePrice ?? 0;
                    var pnLStr = position.PnLUsd.HasValue ? $"{Math.Round(position.PnLUsd.Value, 4)}$" : $"{Math.Round(pnL, 4)} {quoteAssetStr}";

                    var message =
                        $"{position.AssetPairId}; " +
                        $"PL={pnLStr}; " +
                        $"{(position.Type == PositionType.Short ? "Sell" : "Buy")}; " +
                        $"Volume: {Math.Round(position.Volume, 6)}; " +
                        $"OpenPrice: {Math.Round(position.Price, 6)}; " +
                        $"ClosePrice: {Math.Round(closePrice, 6)}; " +
                        $"Close: {position.CloseDate:MM-dd HH:mm:ss}; " +
                        $"Markup: {(position.Type == PositionType.Short ? markup.TotalAskMarkup : markup.TotalBidMarkup)}";

                    if (positions.Count >= 470) message += "; !!!max count of position in day, please add limit!!!";
                    await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId, message);

                    if (position.CloseDate.HasValue)
                        _lastClose[key] = position.CloseDate.Value;

                    countTrade++;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                _lastClose[key] = DateTime.UtcNow;
            }

            Log.Info($"Check api completed. Found: {countTrade} trades. Api: {key}. LastTime: {lastClose:yyyy-MM-dd HH:mm:ss}");
        }

        private ILiquidityEngineClient CreateClient(string url)
        {
            var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(url)
                .WithAdditionalCallsWrapper(new HttpClientGenerator.Infrastructure.ExceptionHandlerCallsWrapper())
                .WithoutRetries()
                .WithoutCaching()
                .Create();

            var client = new LiquidityEngineClient(generator);

            return client;
        }
    }
}
