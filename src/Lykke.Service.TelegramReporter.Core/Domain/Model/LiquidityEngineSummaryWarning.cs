using System;

namespace Lykke.Service.TelegramReporter.Core.Domain.Model
{
    public class LiquidityEngineSummaryWarning : ILiquidityEngineSummaryWarning
    {
        public string AssetPair { get; set; }

        public decimal PnL { get; set; }

        public decimal? PnLInUsd { get; set; }

        public int Count { get; set; }

        public decimal BuyTotal { get; set; }

        public decimal SellTotal { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}
