using System;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services.MarketMakerArbitrages;

namespace Lykke.Service.TelegramReporter.Services.MarketMakerArbitrages
{
    public class MarketMakerArbitragesWarningProvider : IMarketMakerArbitragesWarningProvider
    {
        public Task<string> GetWarningMessageAsync(IMarketMakersArbitragesWarning mmArbitrageses)
        {
            var sb = new StringBuilder();
            var rn = Environment.NewLine;

            sb.AppendLine($"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} ======={rn}{rn}" +
                          $"Market Maker Arbitrages Warning:{rn}");

            sb.AppendLine($"Arbitrages count: {mmArbitrageses.ArbitragesCount}{rn}");

            if (mmArbitrageses.BiggestSpread.HasValue)
            {
                sb.AppendLine($"Biggest spread: {mmArbitrageses.BiggestSpread:0.##}%");
                sb.AppendLine($"{ mmArbitrageses.BiggestSpreadRow}{rn}");
            }

            if (mmArbitrageses.BiggestPnlInUsd.HasValue)
            {
                sb.AppendLine($"Biggest PnL: ${mmArbitrageses.BiggestPnlInUsd:0}");
                sb.AppendLine($"{mmArbitrageses.BiggestPnlInUsdRow}{rn}");
            }

            if (mmArbitrageses.BiggestVolumeInUsd.HasValue)
            {
                sb.AppendLine($"Biggest volume: ${mmArbitrageses.BiggestVolumeInUsd:0}");
                sb.AppendLine($"{mmArbitrageses.BiggestVolumeInUsdRow}{rn}");
            }

            sb.AppendLine($"Most frequent asset pairs:{rn}{mmArbitrageses.MostFrequentAssetPairs}");

            return Task.FromResult(ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString()));
        }
    }
}
