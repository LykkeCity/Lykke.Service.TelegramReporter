using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client;
using Lykke.Service.MarketMakerReports.Client.Models.InventorySnapshots;
using Lykke.Service.TelegramReporter.Core.Services.NettingEngine;
using Lykke.Service.TelegramReporter.Services.NettingEngine.Common;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine
{
    public class NettingEngineStateProvider : INettingEngineStateProvider
    {
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;

        public NettingEngineStateProvider(IAssetsServiceWithCache assetsServiceWithCache)
        {
            _assetsServiceWithCache = assetsServiceWithCache;
        }

        public async Task<string> GetStateMessageAsync(InventorySnapshotModel prevSnapshot, InventorySnapshotModel snapshot)
        {
            var messageText = await GetStateMessage(prevSnapshot, snapshot);
            return ChatMessageHelper.CheckSizeAndCutMessageIfNeeded(messageText);
        }

        private async Task<string> GetStateMessage(InventorySnapshotModel prevSnapshot, InventorySnapshotModel snapshot)
        {
            var prevInventory = await InventoryHelper.GetAssetInventory(prevSnapshot, _assetsServiceWithCache);
            var inventory = await InventoryHelper.GetAssetInventory(snapshot, _assetsServiceWithCache);

            var messageText = new StringBuilder();

            messageText.AppendLine($"======= {snapshot.Timestamp:yyyy/MM/dd HH:mm:ss} =======");
            messageText.AppendLine($"=== from {prevSnapshot.Timestamp:yyyy/MM/dd HH:mm:ss} ===\r\n");
            messageText.AppendLine("Netting Engine State:\r\n");

            var inventoryBalancePnL = 0m;
            var inventoryPnL = 0m;
            var capitalPnL = 0m;
            var oldInventoryPnL = 0m;

            var assetIds = prevInventory.Rows.Select(x => x.Asset.Id).Union(inventory.Rows.Select(x => x.Asset.Id)).Distinct();

            foreach (var assetId in assetIds)
            {
                var prevRow = prevInventory.Rows.SingleOrDefault(x => x.Asset.Id == assetId);
                var row = inventory.Rows.SingleOrDefault(x => x.Asset.Id == assetId);

                var assetBalance1 = prevRow?.Summary?.Balance ?? 0;
                var assetBalance2 = row?.Summary?.Balance ?? 0;
                var assetTurnover1 = (prevRow?.Summary?.Sell ?? 0) + (prevRow?.Summary?.Buy ?? 0);
                var assetTurnover2 = (row?.Summary?.Sell ?? 0) + (row?.Summary?.Buy ?? 0);

                var assetInventory1 = prevRow?.Summary?.Inventory ?? 0;
                var assetInventory2 = row?.Summary?.Inventory ?? 0;

                var assetInventoryByBalance = assetBalance2 - assetBalance1;
                var assetInventory = assetInventory2 - assetInventory1;
                var assetTurnover = assetTurnover2 - assetTurnover1;

                var assetPrice1 = 0m;
                if (assetBalance1 != 0)
                {
                    assetPrice1 = (prevRow?.Summary?.BalanceUsd ?? 0m) / assetBalance1;
                }

                var assetPrice2 = 0m;
                if (assetBalance2 != 0)
                {
                    assetPrice2 = (row?.Summary?.BalanceUsd ?? 0m) / assetBalance2;
                }

                var price = assetPrice1 != 0 
                    ? (assetPrice2 - assetPrice1) / assetPrice1 * 100m 
                    : 0m;

                messageText.AppendLine(
                    $"{row?.Asset?.Title ?? prevRow?.Asset?.Title}; " +
                    $"Inv(bl): {assetInventoryByBalance:+0.00;-0.00;0.00}; " +
                    $"Inv(tr): {assetInventory:+0.00;-0.00;0.00}; " +
                    $"Tur: {assetTurnover:+0.00;-0.00;0.00}; " +
                    $"Price $: {price:+0.00;-0.00;0.00}%"
                );

                inventoryBalancePnL += assetInventoryByBalance * assetPrice2;
                inventoryPnL += assetInventory * assetPrice2;
                capitalPnL += assetBalance2 * assetPrice2 - assetBalance1 * assetPrice1;
                oldInventoryPnL += (row?.Summary?.Inventory ?? 0) * assetPrice2;
            }

            messageText.AppendLine();

            messageText.AppendLine($"Inv (by balance) PnL: {inventoryBalancePnL:+0.00;-0.00;0.00}$");
            messageText.AppendLine($"Inv (by trade) PnL: {inventoryPnL:+0.00;-0.00;0.00}$");
            messageText.AppendLine($"Capital PnL: {capitalPnL:+0.00;-0.00;0.00}$");
            //messageText.AppendLine($"Old Inv PnL: {oldInventoryPnL:+0.00;-0.00;0.00}$");

            return messageText.ToString();
        }
    }
}
