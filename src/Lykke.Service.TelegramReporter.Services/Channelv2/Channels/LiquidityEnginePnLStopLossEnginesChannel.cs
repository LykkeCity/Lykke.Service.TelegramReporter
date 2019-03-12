using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossEngines;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;
using Lykke.Service.TelegramReporter.Services.LiquidityEngine;

namespace Lykke.Service.TelegramReporter.Services.Channelv2.Channels
{
    public class LiquidityEnginePnLStopLossEnginesChannel : ReportChannel
    {
        private readonly LiquidityEngineUrlSettings _settings;
        private readonly Dictionary<string, IPnLStopLossEnginesApi> _clients = new Dictionary<string, IPnLStopLossEnginesApi>();

        public const string Name = "LiquidityEnginePnLStopLossEngines";

        public LiquidityEnginePnLStopLossEnginesChannel(
            IReportChannel channel,
            ITelegramSender telegramSender,
            LiquidityEngineUrlSettings settings,
            ILogFactory logFactory)
            : base(channel, telegramSender, logFactory)
        {
            _settings = settings;
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
                await Execute(client.Key, client.Value);
            }
        }

        private async Task Execute(string key, IPnLStopLossEnginesApi pnLStopLossEnginesApi)
        {
            Log.Info($"Started checking pnl stop loss engines for LE with key '{key}'.");

            var sb = new StringBuilder();
            sb.AppendLine("Liquidity Engine PnL Stop Loss Engines");

            try
            {
                IReadOnlyCollection<PnLStopLossEngineModel> pnLStopLossEngines = await pnLStopLossEnginesApi.GetAllAsync();

                pnLStopLossEngines = pnLStopLossEngines.Where(x => x.Mode == PnLStopLossEngineMode.Active).ToList();

                if (!pnLStopLossEngines.Any())
                    return;
                
                foreach (var engine in pnLStopLossEngines)
                {
                    var passedSinceStart = DateTime.UtcNow - engine.LastTime.Value;
                    var expectedTime = DateTime.UtcNow + engine.Interval - passedSinceStart;
                    var remainingTime = expectedTime - DateTime.UtcNow;

                    var markup = (engine.Markup * 100).ToString("0.##");

                    sb.AppendLine($"{engine.AssetPairId}: Threshold={engine.Threshold}$, Interval={engine.Interval}, Markup={markup}%. RemainingTime: {remainingTime:hh\\:mm\\:ss}.");
                }

                await SendMessage(sb.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Log.Info($"Finished checking pnl stop loss engines for LE with key '{key}'.");
        }

        private IPnLStopLossEnginesApi CreateApiClient(string url)
        {
            var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(url)
                .WithAdditionalCallsWrapper(new HttpClientGenerator.Infrastructure.ExceptionHandlerCallsWrapper())
                .WithoutRetries()
                .WithoutCaching()
                .Create();

            return generator.Generate<IPnLStopLossEnginesApi>();
        }
    }
}
