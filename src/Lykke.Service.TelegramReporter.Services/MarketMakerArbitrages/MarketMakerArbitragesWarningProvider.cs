using System;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services.MarketMakerArbitrages;

namespace Lykke.Service.TelegramReporter.Services.MarketMakerArbitrages
{
    public class MarketMakerArbitragesWarningProvider : IMarketMakerArbitragesWarningProvider
    {
        public Task<string> GetWarningMessageAsync(IMarketMakersArbitragesWarning mmArbitrages)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"====== {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} ======");
            sb.Append(Environment.NewLine);
            sb.AppendLine("Market Maker Arbitrages Warning");
            sb.Append(Environment.NewLine);
            sb.AppendLine($"Total: {mmArbitrages.ArbitragesCount}");

            if (mmArbitrages.BiggestSpread.HasValue)
            {
                sb.AppendLine($"by Spread: {mmArbitrages.BiggestSpread:0.##}%");
                sb.AppendLine($"{ mmArbitrages.BiggestSpreadRow}");
            }

            if (mmArbitrages.BiggestPnlInUsd.HasValue)
            {
                sb.AppendLine($"by PnL: ${mmArbitrages.BiggestPnlInUsd:0}");
                sb.AppendLine($"{mmArbitrages.BiggestPnlInUsdRow}");
            }

            if (mmArbitrages.BiggestVolumeInUsd.HasValue)
            {
                sb.AppendLine($"by Volume: ${mmArbitrages.BiggestVolumeInUsd:0}");
                sb.AppendLine($"{mmArbitrages.BiggestVolumeInUsdRow}");
            }

            sb.Append(Environment.NewLine);
            sb.AppendLine($"Most used asset pairs:");
            sb.AppendLine($"{mmArbitrages.MostFrequentAssetPairs}");

            return Task.FromResult(ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString()));
        }
    }
}
