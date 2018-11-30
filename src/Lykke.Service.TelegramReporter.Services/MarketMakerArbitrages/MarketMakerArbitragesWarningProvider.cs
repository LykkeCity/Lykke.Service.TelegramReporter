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

            sb.AppendLine($"====== {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} ======");
            sb.AppendLine("Market Maker Arbitrages Warning");
            sb.AppendLine($"Total: {mmArbitrageses.ArbitragesCount}");

            if (mmArbitrageses.BiggestSpread.HasValue)
            {
                sb.AppendLine($"by Spread: {mmArbitrageses.BiggestSpread:0.##}%");
                sb.AppendLine($"{ mmArbitrageses.BiggestSpreadRow}");
            }

            if (mmArbitrageses.BiggestPnlInUsd.HasValue)
            {
                sb.AppendLine($"by PnL: ${mmArbitrageses.BiggestPnlInUsd:0}");
                sb.AppendLine($"{mmArbitrageses.BiggestPnlInUsdRow}");
            }

            if (mmArbitrageses.BiggestVolumeInUsd.HasValue)
            {
                sb.AppendLine($"by Volume: ${mmArbitrageses.BiggestVolumeInUsd:0}");
                sb.AppendLine($"{mmArbitrageses.BiggestVolumeInUsdRow}");
            }

            sb.AppendLine($"Most frequent asset pairs: {mmArbitrageses.MostFrequentAssetPairs}");

            return Task.FromResult(ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString()));
        }
    }
}
