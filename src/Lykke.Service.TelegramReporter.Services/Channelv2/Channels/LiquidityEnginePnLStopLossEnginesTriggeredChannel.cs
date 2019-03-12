using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Client;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossEngines;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;
using Lykke.Service.TelegramReporter.Services.LiquidityEngine;

namespace Lykke.Service.TelegramReporter.Services.Channelv2.Channels
{
    public class LiquidityEnginePnLStopLossEnginesTriggeredChannel : ReportChannel
    {
        private readonly LiquidityEngineUrlSettings _settings;
        private readonly Dictionary<string, ILiquidityEngineClient> _clients = new Dictionary<string, ILiquidityEngineClient>();
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
                    var client = CreateClient(url);
                    _clients.Add(url, client);
                }
            }

            foreach (var client in _clients)
            {
                await Execute(client.Key, client.Value);
            }
        }

        private async Task Execute(string key, ILiquidityEngineClient client)
        {
            Log.Info($"Started checking pnl stop loss engines triggered for LE with key '{key}'.");

            var sb = new StringBuilder();
            sb.AppendLine("Liquidity Engine PnL Stop Loss Engines Triggered");

            var newTriggered = new List<PnLStopLossEngineModel>();

            try
            {
                IReadOnlyList<PnLStopLossEngineModel> current = (await client.PnLStopLossEngines.GetAllAsync()).ToList();

                current = current.Where(x => x.Mode == PnLStopLossEngineMode.Active).ToList();

                lock (_sync)
                {
                    if (!_lastEnginesStates.ContainsKey(key))
                        newTriggered.AddRange(current);
                    else
                    {
                        var previous = _lastEnginesStates[key];

                        foreach (var engine in current)
                        {
                            if (previous.Any(x => x.Id == engine.Id))
                                continue;

                            newTriggered.Add(engine);
                        }
                    }

                    _lastEnginesStates[key] = current;
                }

                if (!newTriggered.Any())
                    return;

                foreach (var engine in newTriggered)
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

            Log.Info($"Finished checking pnl stop loss engines triggered for LE with key '{key}'.");
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
