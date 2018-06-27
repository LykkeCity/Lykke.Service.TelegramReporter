using Lykke.Service.CrossMarketLiquidity.Client;
using Lykke.Service.CrossMarketLiquidity.Client.AutorestClient.Models;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlSummaryProvider : ICmlSummaryProvider
    {
        private ICrossMarketLiquidityInstanceManager _crossMarketLiquidityInstanceManager;

        public CmlSummaryProvider(ICrossMarketLiquidityInstanceManager crossMarketLiquidityInstanceManager)
        {
            _crossMarketLiquidityInstanceManager = crossMarketLiquidityInstanceManager;
        }

        public async Task<string> GetSummaryMessageAsync()
        {
            var tasks = new List<Task<string>>();
            foreach (var instanceId in _crossMarketLiquidityInstanceManager.Keys)
            {
                tasks.Add(GetSummaryMessageAsync(instanceId));
            }

            await Task.WhenAll(tasks);

            var sb = new StringBuilder();
            foreach (Task<string> task in tasks)
            {
                sb.AppendLine(task.Result);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public async Task<string> GetSummaryMessageAsync(string instanceId)
        {
            ICrossMarketLiquidityClient client = _crossMarketLiquidityInstanceManager[instanceId];
            var inventoryStateTask = client.GetInventoryStateAsync();
            var balancesTask = client.GetBalancesAsync();

            await Task.WhenAll(inventoryStateTask, balancesTask);

            return GetSummaryMessage(instanceId, inventoryStateTask.Result, balancesTask.Result);
        }

        private string GetSummaryMessage(string instanceId, InventoryStateDto inventoryState,
            BalanceListDto balances)
        {
            const string instanceIdHeader = "Name";
            const string inventoryHeader = "Inventory";
            const string volumeTotalHeader = "VolumeTotal";
            const string realizedPnLHeader = "RealizedPnL";
            const string unrealizedPnLHeader = "UnrealizedPnL";
            const string lykkeBalancesHeader = "Lykke Balances";
            const string externalBalancesHeader = "External Balances";

            double volumeTotal = 0;
            if (inventoryState != null)
            {
                volumeTotal = (inventoryState.RealizedBuyVolumeTotal + inventoryState.RealizedSellVolumeTotal);
            }

            return "Cross Market Liquidity Summary:\r\n\r\n" +
                   $"{instanceIdHeader}: {instanceId}\r\n" +
                   $"{inventoryHeader}: {inventoryState?.AbsoluteInventory ?? 0}\r\n" +
                   $"{volumeTotalHeader}: {volumeTotal}\r\n" +
                   $"{realizedPnLHeader}: {inventoryState?.RealizedPnL ?? 0:0.####}\r\n" +
                   $"{unrealizedPnLHeader}: {inventoryState?.LykkeUnrealizedPnl ?? 0:0.####}\r\n" +
                   $"{lykkeBalancesHeader}: {GetBalancesValue(balances?.LykkeBalance)}\r\n" +
                   $"{externalBalancesHeader}: {GetBalancesValue(balances?.ExternalBalance)}";
        }

        private string GetBalancesValue(AssetPairBalancesDto balances)
        {
            if (balances == null)
            {
                return "No balances";
            }

            return $"{balances.Asset.BaseAsset}:{balances.BaseAssetBalance}" +
                $" {balances.Asset.QuoteAsset}:{balances.QuoteAssetBalance}";
        }
    }
}
