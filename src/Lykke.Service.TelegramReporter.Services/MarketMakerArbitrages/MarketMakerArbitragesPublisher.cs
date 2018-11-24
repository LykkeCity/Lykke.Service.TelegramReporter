using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.MarketMakerArbitrageDetector.Client;
using Lykke.Service.TelegramReporter.Core;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.MarketMakerArbitrages;
using MoreLinq;

namespace Lykke.Service.TelegramReporter.Services.MarketMakerArbitrages
{
    public class MarketMakerArbitragesPublisher : ChatPublisher
    {
        private int PreviousCount { get; set; }

        private readonly IMarketMakerArbitragesWarningProvider _marketMakerArbitragesWarningProvider;
        private readonly IMarketMakerArbitrageDetectorClient _marketMakerArbitrageDetectorClient;

        public MarketMakerArbitragesPublisher(ITelegramSender telegramSender, IChatPublisherSettings publisherSettings,
            IMarketMakerArbitragesWarningProvider marketMakerArbitragesWarningProvider,
            IMarketMakerArbitrageDetectorClient marketMakerArbitrageDetectorClient, ILogFactory logFactory)
            : base(telegramSender, publisherSettings, logFactory)
        {
            _marketMakerArbitragesWarningProvider = marketMakerArbitragesWarningProvider;
            _marketMakerArbitrageDetectorClient = marketMakerArbitrageDetectorClient;
        }

        public override async void Publish()
        {
            try
            {
                var warning = await GetWarningIfExists();
                if (warning == null)
                    return;

                await TelegramSender.SendTextMessageAsync(PublisherSettings.ChatId,
                    await _marketMakerArbitragesWarningProvider.GetWarningMessageAsync(warning));
            }
            catch (Exception ex)
            {
                await Log.WriteErrorAsync(nameof(MarketMakerArbitragesPublisher), nameof(Publish), "", ex);
            }
        }

        private async Task<IMarketMakersArbitragesWarning> GetWarningIfExists()
        {
            var arbitrages = await _marketMakerArbitrageDetectorClient.Arbitrages.GetAllAsync(null, null); // no filtration

            if (PreviousCount != 0 || arbitrages.Count < 1)
                return null;

            PreviousCount = arbitrages.Count;

            var assetPairs = new List<string>(arbitrages.Select(x => x.Target.Name));
            assetPairs.AddRange(arbitrages.SelectMany(x => x.ConversionPath.Split(" & ")));
            var grouped = assetPairs.GroupBy(x => x).OrderByDescending(x => x.Count()).Take(5);
            var mostFrequent = string.Join(", ", grouped.Select(x => $"{x.Key}({x.Count()})"));

            var biggestSpread = arbitrages.Count(x => x.Spread < 0) > 0
                ? arbitrages.MinBy(x => x.Spread) : null;
            var biggestSpreadRow =  $"{biggestSpread?.Target} - {biggestSpread?.Source}  ({biggestSpread?.ConversionPath})";

            var biggestPnlInUsd = arbitrages.Count(x => x.PnLInUsd.HasValue && x.PnLInUsd.Value > 0) > 0
                ? arbitrages.MaxBy(x => x.PnLInUsd) : null;
            var biggestPnlInUsdRow = $"{biggestPnlInUsd?.Target} - {biggestPnlInUsd?.Source}  ({biggestPnlInUsd?.ConversionPath})";

            var biggestVolumeInUsd = arbitrages.Count(x => x.VolumeInUsd.HasValue && x.VolumeInUsd.Value > 0) > 0
                ? arbitrages.MaxBy(x => x.VolumeInUsd) : null;
            var biggestVolumeInUsdRow = $"{biggestVolumeInUsd?.Target} - {biggestVolumeInUsd?.Source}  ({biggestVolumeInUsd?.ConversionPath})";

            return new MarketMakersArbitragesWarning
            {
                ArbitragesCount = arbitrages.Count,

                BiggestSpread = biggestSpread?.Spread,
                BiggestSpreadRow = biggestSpreadRow,

                BiggestPnlInUsd = biggestPnlInUsd?.PnLInUsd,
                BiggestPnlInUsdRow = biggestPnlInUsdRow,

                BiggestVolumeInUsd = biggestVolumeInUsd?.VolumeInUsd,
                BiggestVolumeInUsdRow = biggestVolumeInUsdRow,
                
                MostFrequentAssetPairs = mostFrequent
            };
        }
    }
}
