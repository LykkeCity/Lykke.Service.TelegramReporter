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
    public class LiquidityEnginePnLStopLossEnginesTriggeredChannel : ReportChannel
    {
        private readonly LiquidityEngineUrlSettings _settings;
        private readonly Dictionary<string, IPnLStopLossEnginesApi> _clients
            = new Dictionary<string, IPnLStopLossEnginesApi>();
        private readonly object _sync = new object();
        private readonly Dictionary<string, IReadOnlyList<PnLStopLossEngineModel>> _lastEnginesStates
            = new Dictionary<string, IReadOnlyList<PnLStopLossEngineModel>>();

        public const string Name = "LiquidityEnginePnLStopLossEnginesTriggered";

        public LiquidityEnginePnLStopLossEnginesTriggeredChannel(
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
            Log.Info($"Started checking pnl stop loss engines triggered for LE with key '{key}'.");

            var sb = new StringBuilder();
            sb.AppendLine("Liquidity Engine PnL Stop Loss Engines Triggered");

            var newTriggered = new List<PnLStopLossEngineModel>();

            try
            {
                IReadOnlyList<PnLStopLossEngineModel> pnLStopLossEngines = (await pnLStopLossEnginesApi.GetAllAsync()).ToList();

                pnLStopLossEngines = pnLStopLossEngines.Where(x => x.Mode == PnLStopLossEngineMode.Active).ToList();

                if (!pnLStopLossEngines.Any())
                    return;

                lock (_sync)
                {
                    if (!_lastEnginesStates.ContainsKey(key))
                    {
                        _lastEnginesStates[key] = pnLStopLossEngines;

                        return;
                    }

                    var previous = _lastEnginesStates[key];

                    foreach (var engine in pnLStopLossEngines)
                    {
                        if (previous.Any(x => x.Id == engine.Id))
                            continue;

                        newTriggered.Add(engine);
                    }

                    _lastEnginesStates[key] = pnLStopLossEngines;
                }

                if (!newTriggered.Any())
                    return;

                foreach (var engine in newTriggered)
                {
                    var expectedTime = DateTime.UtcNow - (engine.LastTime + engine.Interval);

                    sb.AppendLine($"{engine.AssetPairId}: Threshold={engine.Threshold}, Interval={engine.Interval}. ExpectedTime: {expectedTime}.");
                }

                await SendMessage(sb.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Log.Info($"Finished checking pnl stop loss engines triggered for LE with key '{key}'.");
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
