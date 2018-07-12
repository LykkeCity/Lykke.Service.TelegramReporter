using System;
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
        private readonly ICrossMarketLiquidityInstanceManager _crossMarketLiquidityInstanceManager;

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
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(sb.ToString());
        }

        public async Task<string> GetStateMessageAsync(string instanceId)
        {
            ICrossMarketLiquidityClient client = _crossMarketLiquidityInstanceManager[instanceId];
            var tradingAlgorithmPropertiesTask = client.GetTradingAlgorithmPropertiesAsync();
            var inventoryStateTask = client.GetInventoryStateAsync();
            var settingsTask = client.GetCMLSettingsAsync();

            await Task.WhenAll(tradingAlgorithmPropertiesTask, inventoryStateTask, settingsTask);

            var message = GetStateMessage(instanceId, tradingAlgorithmPropertiesTask.Result, inventoryStateTask.Result, settingsTask.Result);
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(message);
        }

        private string GetStateMessage(string instanceId,
            TradingAlgorithmPropertiesDto tradingAlgorithmProperties,
            InventoryStateDto inventoryState,
            CMLSettingsDto settings)
        {
            const string instanceIdHeader = "Name";

            return $"======= {DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} =======\r\n\r\n" +
                   "Cross Market Liquidity State:\r\n\r\n" +
                   $"{instanceIdHeader}: {instanceId}\r\n" +
                   $"{nameof(TradingAlgorithmPropertiesDto.Mode)}: {tradingAlgorithmProperties.Mode}\r\n" +
                   $"{nameof(TradingAlgorithmPropertiesDto.Status)}: {tradingAlgorithmProperties.Status}\r\n\r\n" +
                   $"Lykke {inventoryState.LykkeBalances.Asset.BaseAsset}: {inventoryState.LykkeBalances.BaseAssetBalance}\r\n" +
                   $"Lykke {inventoryState.LykkeBalances.Asset.QuoteAsset}: {inventoryState.LykkeBalances.QuoteAssetBalance}\r\n\r\n" +
                   $"{settings.ExternalAssetPair.Exchange} {inventoryState.ExternalBalances.Asset.BaseAsset}: {inventoryState.ExternalBalances.BaseAssetBalance}\r\n" +
                   $"{settings.ExternalAssetPair.Exchange} {inventoryState.ExternalBalances.Asset.QuoteAsset}: {inventoryState.ExternalBalances.QuoteAssetBalance}\r\n\r\n" +
                   $"{nameof(InventoryStateDto.AbsoluteInventory)}: {inventoryState.AbsoluteInventory}\r\n" +
                   $"{nameof(InventoryStateDto.RealizedPnL)}: {Math.Round(inventoryState.RealizedPnL, 4)}\r\n" +
                   $"{nameof(InventoryStateDto.LykkeUnrealizedPnl)}: {Math.Round(inventoryState.LykkeUnrealizedPnl, 4)}\r\n" +
                   $"{nameof(InventoryStateDto.ExternalUnrealizedPnl)}: {Math.Round(inventoryState.ExternalUnrealizedPnl, 4)}\r\n" +
                   $"{nameof(InventoryStateDto.RealizedBuyVolumeTotal)}: {inventoryState.RealizedBuyVolumeTotal}\r\n" +
                   $"{nameof(InventoryStateDto.RealizedSellVolumeTotal)}: {inventoryState.RealizedSellVolumeTotal}";
        }
    }
}
