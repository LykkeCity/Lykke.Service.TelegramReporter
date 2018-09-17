namespace Lykke.Service.TelegramReporter.Core.Domain.Model
{
    public interface IMarketMakersArbitragesWarning
    {
        int ArbitragesCount { get; }

        decimal? BiggestSpread { get; }
        string BiggestSpreadRow { get; }

        decimal? BiggestPnlInUsd { get; }
        string BiggestPnlInUsdRow { get; }

        decimal? BiggestVolumeInUsd { get; }
        string BiggestVolumeInUsdRow { get; }

        string MostFrequentAssetPairs { get; }
    }
}
