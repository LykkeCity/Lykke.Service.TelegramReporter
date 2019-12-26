namespace Lykke.Service.TelegramReporter.Core.Domain.Model
{
    public class MarketMakersArbitragesWarning : IMarketMakersArbitragesWarning
    {
        public int ArbitragesCount { get; set; }

        public decimal? BiggestSpread { get; set; }
        public string BiggestSpreadRow { get; set; }

        public decimal? BiggestPnlInUsd { get; set; }
        public string BiggestPnlInUsdRow { get; set; }

        public decimal? BiggestVolumeInUsd { get; set; }
        public string BiggestVolumeInUsdRow { get; set; }

        public string MostFrequentAssetPairs { get; set; }

        public string MarketMakers { get; set; }
    }
}
