using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.MarketMakerReports.Client;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineTradesChatPublisher : ChatPublisher
    {
        private readonly IMarketMakerReportsFiatClient _marketMakerReportsClient;

        private DateTime _lastTradeTime = DateTime.UtcNow;

        public NettingEngineTradesChatPublisher(
            IMarketMakerReportsFiatClient marketMakerReportsClient,
            ITelegramSender telegramSender, 
            IChatPublisherSettings publisherSettings, 
            ILogFactory logFactory) 
            : base(telegramSender, publisherSettings, logFactory)
        {
            _marketMakerReportsClient = marketMakerReportsClient;
        }

        public override async Task Publish()
        {
            var client = _marketMakerReportsClient.Client.LykkeTradesApi;

            var dataraw = await client.GetAsync(_lastTradeTime, DateTime.UtcNow, null, null);

            var data = dataraw.Entities.OrderBy(e => e.Time).ToList();

            try
            {
                foreach (var trade in data)
                {
                    var message = $"{trade.AssetPairId}, {trade.Type.ToString()}, price: {trade.Price}, Volume: {trade.Volume}; {trade.Time:HH:mm:ss}";
                    await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId, message);
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error at sent trade message", exception: ex);
            }


            if (data.Any())
            {
                _lastTradeTime = data.Max(e => e.Time);
            }
        }
    }
}
