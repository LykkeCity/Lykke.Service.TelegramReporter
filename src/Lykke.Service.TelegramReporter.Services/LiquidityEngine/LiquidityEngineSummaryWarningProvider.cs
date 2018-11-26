using System;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Domain.Model;
using Lykke.Service.TelegramReporter.Core.Services.LiquidityEngine;

namespace Lykke.Service.TelegramReporter.Services.LiquidityEngine
{
    public class LiquidityEngineSummaryWarningProvider : ILiquidityEngineSummaryWarningProvider
    {
        public Task<string> GetWarningMessageAsync(ILiquidityEngineSummaryWarning warning)
        {
            var sb = new StringBuilder();
            var rn = Environment.NewLine;

            sb.AppendLine($"=== {warning.To:yyyy/MM/dd HH:mm:ss} (end date) ==={rn}" +
                          $"=== {warning.From:yyyy/MM/dd HH:mm:ss} (start date) ==={rn}" +
                          $"Liquidity Engine Statistic{rn}");

            var pnL = warning.PnLInUsd.HasValue ? $"{warning.PnLInUsd.Value}$ [{warning.PnL}]" : $"{warning.PnL} [quote asset]";

            sb.AppendLine($"{warning.AssetPair}; PL: {pnL}; Count: {warning.Count}; Sell: {warning.SellTotal}; Buy: {warning.BuyTotal};");

            return Task.FromResult(ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString()));
        }
    }
}
