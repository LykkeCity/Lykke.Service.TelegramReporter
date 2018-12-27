using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.CryptoIndex.Client.Api;
using Lykke.Service.CryptoIndex.Client.Models;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Services.CryptoIndex.InstancesSettings;

namespace Lykke.Service.TelegramReporter.Services.CryptoIndex
{
    public class CryptoIndexWarningsPublisher : ChatPublisher
    {
        private readonly CryptoIndexInstancesSettings _settings;
        private readonly Dictionary<string, IWarningsApi> _clients = new Dictionary<string, IWarningsApi>();
        private readonly Dictionary<string, DateTime> _lastTimes = new Dictionary<string, DateTime>();

        public CryptoIndexWarningsPublisher(ITelegramSender telegramSender, IChatPublisherSettings publisherSettings,
            CryptoIndexInstancesSettings settings, ILogFactory logFactory)
            : base(telegramSender, publisherSettings, logFactory)
        {
            _settings = settings;
        }

        public override async Task Publish()
        {
            if (!_clients.Any())
            {
                foreach (var instance in _settings.Instances)
                {
                    var client = CreateApiClient(instance.ServiceUrl);
                    _clients.Add(instance.DisplayName, client);
                }
            }

            foreach (var reportsApi in _clients)
            {
                await CheckApi(reportsApi.Key, reportsApi.Value);
            }
        }
        
        private async Task CheckApi(string indexName, IWarningsApi warningsApi)
        {
            var fromDate = DateTime.UtcNow;
            var toDate = DateTime.UtcNow;
            
            if (!_lastTimes.TryGetValue(indexName, out var lastTime))
            {
                lastTime = DateTime.UtcNow;
                _lastTimes[indexName] = lastTime;
            }

            Log.Info($"Started requesting warning history. Api: {indexName}. LastTime: {lastTime:yyyy-MM-dd HH:mm:ss}");

            IReadOnlyCollection<Warning> warnings = new List<Warning>();
            try
            {
                warnings = await warningsApi.GetHistoryAsync(fromDate, toDate);

                foreach (var warning in warnings)
                {
                    var message = $"{indexName}: {warning.Message}";

                    await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId, message);

                    _lastTimes[indexName] = warning.Time;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                _lastTimes[indexName] = DateTime.UtcNow;
            }

            Log.Info($"Finished request for warning history. Found: {warnings.Count} warnings. Api: {indexName}. LastTime: {lastTime:yyyy-MM-dd HH:mm:ss}");
        }

        private IWarningsApi CreateApiClient(string url)
        {
            var generator = HttpClientGenerator.HttpClientGenerator.BuildForUrl(url)
                .WithAdditionalCallsWrapper(new HttpClientGenerator.Infrastructure.ExceptionHandlerCallsWrapper())
                .WithoutRetries()
                .WithoutCaching()
                .Create();

            var client = generator.Generate<IWarningsApi>();

            return client;
        }
    }
}
