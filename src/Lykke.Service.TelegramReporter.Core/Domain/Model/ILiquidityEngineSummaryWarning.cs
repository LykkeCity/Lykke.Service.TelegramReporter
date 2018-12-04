using System;

namespace Lykke.Service.TelegramReporter.Core.Domain.Model
{
    public interface ILiquidityEngineSummaryWarning
    {
        string AssetPair { get; }

        decimal PnL { get; }

        decimal? PnLInUsd { get; }

        int Count { get; }

        decimal BuyTotal { get; }

        decimal SellTotal { get; }

        DateTime From { get; }

        DateTime To { get; }
    }
}
