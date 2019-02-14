using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.InstrumentMessages;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;
using Lykke.Service.TelegramReporter.Services.LiquidityEngine;

namespace Lykke.Service.TelegramReporter.Services.Channelv2.Channels
{
    public class LiquidityEngineMessagesChannel : ReportChannel
    {
        private readonly LiquidityEngineUrlSettings _settings;
        private readonly Dictionary<string, IInstrumentMessagesApi> _clients = new Dictionary<string, IInstrumentMessagesApi>();

        public const string Name = "LiquidityEngineMessages";

        public LiquidityEngineMessagesChannel(
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

        private async Task Execute(string key, IInstrumentMessagesApi instrumentMessagesApi)
        {
            Log.Info($"Started checking instrument messages for LE with key '{key}'.");

            var sb = new StringBuilder();
            sb.AppendLine("Liquidity Engine Instrument Messages");

            try
            {
                IReadOnlyCollection<InstrumentMessagesModel> instrumentMessages = await instrumentMessagesApi.GetAllAsync();

                instrumentMessages = instrumentMessages.Where(x => x.Messages.Any()).ToList();

                foreach (var messages in instrumentMessages)
                {
                    sb.AppendLine($"{messages.AssetPairId}: {string.Join(", ", messages.Messages)}");
                }

                await SendMessage(sb.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Log.Info($"Finished checking instrument messages for LE with key '{key}'.");
        }

        private IInstrumentMessagesApi CreateApiClient(string url)
        {
            var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(url)
                .WithAdditionalCallsWrapper(new HttpClientGenerator.Infrastructure.ExceptionHandlerCallsWrapper())
                .WithoutRetries()
                .WithoutCaching()
                .Create();

            return generator.Generate<IInstrumentMessagesApi>();
        }
    }
}
