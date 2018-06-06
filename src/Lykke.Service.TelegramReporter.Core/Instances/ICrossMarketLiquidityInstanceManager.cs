using Lykke.Service.CrossMarketLiquidity.Client;
using System.Collections.Generic;

namespace Lykke.Service.TelegramReporter.Core.Instances
{
    public interface ICrossMarketLiquidityInstanceManager : IReadOnlyDictionary<string, ICrossMarketLiquidityClient>
    {
    }
}
