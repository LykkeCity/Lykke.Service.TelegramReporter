using System.Threading.Tasks;
using Lykke.Service.TelegramReporter.Core.Services.Balance;

namespace Lykke.Service.TelegramReporter.Services.Balance
{
    public class BalanceWarningProvider : IBalanceWarningProvider
    {
        //private ICrossMarketLiquidityInstanceManager _crossMarketLiquidityInstanceManager;

        public BalanceWarningProvider(/*ICrossMarketLiquidityInstanceManager crossMarketLiquidityInstanceManager*/)
        {
            //_crossMarketLiquidityInstanceManager = crossMarketLiquidityInstanceManager;
        }

        public async Task<string> GetWarningMessageAsync()
        {
            return "";
        }

        public async Task<string> GetWarningMessageAsync(string instanceId)
        {
            return "";
        }
    }
}
