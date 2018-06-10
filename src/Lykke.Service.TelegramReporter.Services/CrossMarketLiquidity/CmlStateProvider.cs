using Lykke.Service.CrossMarketLiquidity.Client;
using Lykke.Service.CrossMarketLiquidity.Client.AutorestClient.Models;
using Lykke.Service.TelegramReporter.Core.Instances;
using Lykke.Service.TelegramReporter.Core.Services.CrossMarketLiquidity;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.TelegramReporter.Services.CrossMarketLiquidity
{
    public class CmlStateProvider : ICmlStateProvider
    {
        private ICrossMarketLiquidityInstanceManager _crossMarketLiquidityInstanceManager;

        public CmlStateProvider(ICrossMarketLiquidityInstanceManager crossMarketLiquidityInstanceManager)
        {
            _crossMarketLiquidityInstanceManager = crossMarketLiquidityInstanceManager;
        }

        public async Task<string> GetStateMessageAsync()
        {
            var tasks = new List<Task<string>>();
            foreach (var instanceId in _crossMarketLiquidityInstanceManager.Keys)
            {
                tasks.Add(GetStateMessageAsync(instanceId));
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

        public async Task<string> GetStateMessageAsync(string instanceId)
        {
            ICrossMarketLiquidityClient client = _crossMarketLiquidityInstanceManager[instanceId];
            var tradingAlgorithmPropertiesTask = client.GetTradingAlgorithmPropertiesAsync();
            var inventoryStateTask = client.GetInventoryStateAsync();

            await Task.WhenAll(tradingAlgorithmPropertiesTask, inventoryStateTask);

            return GetStateMessage(instanceId, tradingAlgorithmPropertiesTask.Result, inventoryStateTask.Result);
        }

        private string GetStateMessage(string instanceId, 
            TradingAlgorithmPropertiesDto tradingAlgorithmProperties, 
            InventoryStateDto inventoryState)
        {
            const string instanceIdHeader = "Name";

            return $"{instanceIdHeader}: {instanceId}\r\n" +
                   "Properties of the Trading Algorithm\r\n" +
                   $"{nameof(TradingAlgorithmPropertiesDto.Mode)}: {tradingAlgorithmProperties.Mode}\r\n" +
                   $"{nameof(TradingAlgorithmPropertiesDto.Status)}: {tradingAlgorithmProperties.Status}\r\n" +
                   "\r\nLykke Balance\r\n" +
                   $"{inventoryState.LykkeBalances.Asset.BaseAsset}: {inventoryState.LykkeBalances.BaseAssetBalance}\r\n" +
                   $"{inventoryState.LykkeBalances.Asset.QuoteAsset}: {inventoryState.LykkeBalances.QuoteAssetBalance}\r\n" +
                   "\r\nExternal Balance\r\n" +
                   $"{inventoryState.ExternalBalances.Asset.BaseAsset}: {inventoryState.ExternalBalances.BaseAssetBalance}\r\n" +
                   $"{inventoryState.ExternalBalances.Asset.QuoteAsset}: {inventoryState.ExternalBalances.QuoteAssetBalance}\r\n" +
                   "\r\nInventory State\r\n" +
                   $"{nameof(InventoryStateDto.AbsoluteInventory)}: {inventoryState.AbsoluteInventory}\r\n" +
                   $"{nameof(InventoryStateDto.RealizedPnL)}: {inventoryState.RealizedPnL}\r\n" +
                   $"{nameof(InventoryStateDto.LykkeUnrealizedPnl)}: {inventoryState.LykkeUnrealizedPnl}\r\n" +
                   $"{nameof(InventoryStateDto.ExternalUnrealizedPnl)}: {inventoryState.ExternalUnrealizedPnl}\r\n" +
                   $"{nameof(InventoryStateDto.RealizedBuyVolumeTotal)}: {inventoryState.RealizedBuyVolumeTotal}\r\n" +
                   $"{nameof(InventoryStateDto.RealizedSellVolumeTotal)}: {inventoryState.RealizedSellVolumeTotal}";
        }
    }
}
