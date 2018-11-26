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
            sb.AppendLine($"=== {toDate:yyyy/MM/dd HH:mm:ss} (end date) ==={rn}");
            sb.AppendLine($"=== {fromDate:yyyy/MM/dd HH:mm:ss} (start date) ==={rn}");
            sb.AppendLine("Liquidity Engine Statistic");

            var countTrade = 0;
            try
            {
                var summaryReport = await reportsApi.GetSummaryReportByPeriodAsync(fromDate, toDate);

                _lastTime = DateTime.UtcNow;

                foreach (var model in summaryReport.OrderBy(s => s.AssetPairId))
                {
                    var pnL = model.PnL;
                    var pnLStr = model.PnLUsd.HasValue ? $"{Math.Round(model.PnLUsd.Value, 4)}$" : $"{Math.Round(pnL, 4)} [quote asset]";

                    var message =
                        $"{model.AssetPairId}; " +
                        $"PL={pnLStr}; " +
                        $"Count: {model.ClosedPositionsCount}; " +
                        $"Sell: {Math.Round(model.TotalSellBaseAssetVolume, 6)}; " +
                        $"Buy: {Math.Round(model.TotalBuyBaseAssetVolume, 6)}; ";

                    sb.AppendLine(message);

                    countTrade++;
                }

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
