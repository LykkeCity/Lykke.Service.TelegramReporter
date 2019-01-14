﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;
using Lykke.Service.TelegramReporter.Services.LiquidityEngine;

namespace Lykke.Service.TelegramReporter.Services.Channelv2.Channels
{
    public class LiquidityEngineSummaryChannel : ReportChannel
    {
        private readonly LiquidityEngineUrlSettings _settings;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly Dictionary<string, Apis> _clients = new Dictionary<string, Apis>();
        private DateTime _lastTime;

        public const string Name = "LiquidityEngineSummary";

        public LiquidityEngineSummaryChannel(
            IReportChannel channel, 
            ITelegramSender telegramSender, 
            ILogFactory logFactory,
            LiquidityEngineUrlSettings settings,
            IAssetsServiceWithCache assetsServiceWithCache) 
            : base(channel, telegramSender, logFactory)
        {
            _settings = settings;
            _assetsServiceWithCache = assetsServiceWithCache;
        }

        protected override async Task DoTimer()
        {
            if (!_clients.Any())
            {
                foreach (var url in _settings.Urls)
                {
                    var client = CreateApiClient(url);
                    _clients.Add(url, client);
                }
            }

            foreach (var client in _clients)
            {
                await CheckApi(client.Key, client.Value.ReportsApi, client.Value.TradesApi);
            }
        }

        private async Task CheckApi(string key, IReportsApi reportsApi, ITradesApi tradesApi)
        {
            if (_lastTime == default(DateTime))
            {
                _lastTime = DateTime.UtcNow.Subtract(Interval);
            }

            var fromDate = _lastTime;
            var toDate = DateTime.UtcNow;

            Log.Info($"Check api Start. summary. Api: {key}. LastTime: {_lastTime:yyyy-MM-dd HH:mm:ss}");

            var sb = new StringBuilder();
            sb.AppendLine($"=== {fromDate:yyyy/MM/dd HH:mm:ss} ===");
            sb.AppendLine($"=== {toDate:yyyy/MM/dd HH:mm:ss} ===");
            sb.Append(Environment.NewLine);
            sb.AppendLine("Liquidity Engine Summary Statistics");
            sb.Append(Environment.NewLine);

            var countTrade = 0;
            try
            {
                var data = await reportsApi.GetPositionsReportAsync(fromDate, toDate, int.MaxValue);
                var report = data
                    .Where(x => x.IsClosed)
                    .Where(x => x.CloseDate > fromDate)
                    .Where(x => x.CloseDate <= toDate)
                    .ToList();

                var summary = await reportsApi.GetBalanceIndicatorsReportAsync();

                //if (report.Count == 0)
                //    return;

                var grouped = report.GroupBy(x => x.AssetPairId).OrderBy(x => x.Key);

                var result = new List<string>();
                var totalPl = 0m;
                var totalTurnover = 0m;
                foreach (var assetPairTrades in grouped)
                {
                    var assetPairId = assetPairTrades.Key;
                    var count = assetPairTrades.Count();
                    var buyVolume = assetPairTrades.Where(x => x.Type == PositionType.Long).Sum(x => x.Volume);
                    var sellVolume = assetPairTrades.Where(x => x.Type == PositionType.Short).Sum(x => x.Volume);
                    var isPnLInUsd = assetPairTrades.All(x => x.PnLUsd.HasValue);
                    var pnLInUsd = isPnLInUsd ? assetPairTrades.Sum(x => x.PnLUsd ?? 0) : (decimal?)null;
                    var pnL = assetPairTrades.Sum(x => x.PnL ?? 0);

                    var assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(assetPairId);
                    var quoteAssetId = assetPair?.QuotingAssetId;
                    var asset = await _assetsServiceWithCache.TryGetAssetAsync(quoteAssetId);
                    var quoteAssetDisplayId = quoteAssetId == null ? null : asset.Id;
                    var quoteAssetStr = string.IsNullOrWhiteSpace(quoteAssetDisplayId) ? "[quote asset]" : quoteAssetDisplayId;

                    var pnLStr = pnLInUsd.HasValue ? $"{Math.Round(pnLInUsd.Value, 4)}$" : $"{Math.Round(pnL, 4)} {quoteAssetStr}";

                    var lastTrade = assetPairTrades.Where(e => e.PnLUsd.HasValue).OrderByDescending(e => e.ClosePrice).FirstOrDefault();
                    var latsUsdPrice = (lastTrade != null && lastTrade.PnLUsd.HasValue && lastTrade.PnL.HasValue)
                        ? lastTrade.PnLUsd.Value / lastTrade.PnL.Value
                        : 0m;
                    decimal turnover = assetPairTrades.Sum(e => e.Volume * e.ClosePrice) ?? 0m;
                    var turnoverUsd = turnover * latsUsdPrice;

                    var assetPairMessage = $"{assetPairId}; " +
                                           $"PL={pnLStr}; " +
                                           $"Count: {count}; " +
                                           $"Sell: {Math.Round(sellVolume, 6)}; " +
                                           $"Buy: {Math.Round(buyVolume, 6)}; " +
                                           $"Turnover: {Math.Round(turnoverUsd, 0)} $; ";

                    totalPl += pnLInUsd ?? 0m;
                    totalTurnover += turnoverUsd;

                    result.Add(assetPairMessage);
                }

                sb.AppendLine(string.Join(Environment.NewLine, result));
                sb.Append($"");
                sb.Append($"Tatal PL: {Math.Round(totalPl, 2)} $, Turnover: {Math.Round(totalTurnover, 2)} $");

                _lastTime = DateTime.UtcNow;

                await SendMessage(sb.ToString());

                var sammuryMessage = $"Equity: {(int) summary.Equity} $.  RiskExposure: {(int) summary.RiskExposure} $";
                await SendMessage(sammuryMessage);

                var fiatSettlement = await tradesApi.GetSettlementTradesAsync();
                if (fiatSettlement.Any(e => !e.IsCompleted))
                {
                    sb.Clear();
                    sb.AppendLine("Detect fiat trades without settlement in lykke: ");
                    foreach (var model in fiatSettlement.Where(e => !e.IsCompleted))
                    {
                        sb.AppendLine($"{model.AssetPair}, {model.Type}, price: {model.Price}, volume: {model.Volume}");
                    }
                    await SendMessage(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Log.Info($"Check api complete. Found: {countTrade} asset pairs. Api: {key}. LastTime: {_lastTime:yyyy-MM-dd HH:mm:ss}");
        }

        private Apis CreateApiClient(string url)
        {
            var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(url)
                .WithAdditionalCallsWrapper(new HttpClientGenerator.Infrastructure.ExceptionHandlerCallsWrapper())
                .WithoutRetries()
                .WithoutCaching()
                .Create();

            Apis api = new Apis();
            api.ReportsApi = generator.Generate<IReportsApi>();
            api.TradesApi = generator.Generate<ITradesApi>();

            return api;
        }

        public struct Apis
        {
            public IReportsApi ReportsApi { get; set; }
            public ITradesApi TradesApi { get; set; }
        }
    }
}