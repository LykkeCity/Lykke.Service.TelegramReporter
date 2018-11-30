using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;

namespace Lykke.Service.TelegramReporter.Services.LiquidityEngine
{
    public class LiquidityEngineSummaryPublisher : ChatPublisher
    {
        private readonly LiquidityEngineUrlSettings _settings;
        private DateTime _lastTime;

        private Dictionary<string, IReportsApi> _clients = new Dictionary<string, IReportsApi>();

        public LiquidityEngineSummaryPublisher(ITelegramSender telegramSender, IChatPublisherSettings publisherSettings,
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

        private async Task CheckApi(string key, IReportsApi reportsApi)
        {
            if (_lastTime == default(DateTime))
            {
                _lastTime = DateTime.UtcNow;
                return;
            }

            var fromDate = _lastTime;
            var toDate = DateTime.UtcNow;

            Log.Info($"Check api Start. summary. Api: {key}. LastTime: {_lastTime:yyyy-MM-dd HH:mm:ss}");

            var sb = new StringBuilder();
            var rn = Environment.NewLine;
            sb.AppendLine($"=== {fromDate:yyyy/MM/dd HH:mm:ss} ===");
            sb.AppendLine($"=== {toDate:yyyy/MM/dd HH:mm:ss} ===");
            sb.Append(Environment.NewLine);
            sb.AppendLine("Liquidity Engine Summary Statistics");
            sb.Append(Environment.NewLine);

            var countTrade = 0;
            try
            {
                var report = (await reportsApi.GetPositionsReportAsync(DateTime.Now, DateTime.Now, int.MaxValue)).Where(x => x.IsClosed).ToList();

                if (report.Count == 0)
                    return;

                var grouped = report.GroupBy(x => x.AssetPairId).OrderBy(x => x.Key);

                var result = new List<string>();
                foreach (var assetPairTrades in grouped)
                {
                    var assetPair = assetPairTrades.Key;
                    var count = assetPairTrades.Count();
                    var buyVolume = assetPairTrades.Where(x => x.Type == PositionType.Long).Sum(x => x.Volume);
                    var sellVolume = assetPairTrades.Where(x => x.Type == PositionType.Short).Sum(x => x.Volume);
                    var isPnLInUsd = assetPairTrades.All(x => x.PnLUsd.HasValue);
                    var pnLInUsd = isPnLInUsd ? assetPairTrades.Sum(x => x.PnLUsd.Value) : (decimal?)null;
                    var pnL = assetPairTrades.Sum(x => x.PnL.Value);
                    var pnLStr = pnLInUsd.HasValue ? $"{Math.Round(pnLInUsd.Value, 4)}$" : $"{Math.Round(pnL, 4)} [quote asset]";

                    var assetPairMessage = $"{assetPair}; " +
                                           $"PL={pnLStr}; " +
                                           $"Count: {count}; " +
                                           $"Sell: {Math.Round(sellVolume, 6)}; " +
                                           $"Buy: {Math.Round(buyVolume, 6)}; ";
                    result.Add(assetPairMessage);
                }

                sb.AppendLine(string.Join(Environment.NewLine, result));

                _lastTime = DateTime.UtcNow;

                await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId, sb.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Log.Info($"Check api complete. Found: {countTrade} asset pairs. Api: {key}. LastTime: {_lastTime:yyyy-MM-dd HH:mm:ss}");
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
