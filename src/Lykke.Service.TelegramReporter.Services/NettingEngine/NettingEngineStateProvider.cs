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
            messageText.AppendLine($"== from {prevSnapshot.Timestamp:yyyy/MM/dd HH:mm:ss} ===\r\n");
            messageText.AppendLine("Netting Engine State:\r\n");

            var inventoryPnL = 0m;
            var capitalPnL = 0m;
            var oldInventoryPnL = 0m;

            foreach (var row in inventory.Rows)
            {
                var prevRow = prevInventory.Rows.SingleOrDefault(x => x.Asset.Id == row.Asset.Id);
                if (prevRow == null)
                {
                    continue;
                }

                var assetBalance1 = prevRow.Summary.Balance;
                var assetBalance2 = row.Summary.Balance;
                var assetTurnover1 = prevRow.Summary.Sell + prevRow.Summary.Buy;
                var assetTurnover2 = row.Summary.Sell + row.Summary.Buy;

                var assetInventory = assetBalance2 - assetBalance1;
                var assetTurnover = assetTurnover2 - assetTurnover1;

                var assetPrice1 = assetBalance1 != 0 ? prevRow.Summary.BalanceUsd / assetBalance1 : 0;
                var assetPrice2 = assetBalance2 != 0 ? row.Summary.BalanceUsd / assetBalance2 : 0;

                var price = (assetPrice2 - assetPrice1) / assetPrice1 * 100;

                messageText.AppendLine(
                    $"{row.Asset.Title}; " +
                    $"Inv: {assetInventory:+0.00;-0.00;0.00}; " +
                    $"Tur: {assetTurnover:+0.00;-0.00;0.00}; " +
                    $"Price $: {price:+0.00;-0.00;0.00}%"
                );

                inventoryPnL += assetInventory * assetPrice2;
                capitalPnL += assetBalance2 * assetPrice2 - assetBalance1 * assetPrice1;
                oldInventoryPnL += row.Summary.Inventory * assetPrice2;
            }

            messageText.AppendLine();

            messageText.AppendLine($"Inv PnL: {inventoryPnL:+0.00;-0.00;0.00}$");
            messageText.AppendLine($"Capital PnL: {capitalPnL:+0.00;-0.00;0.00}$");
            messageText.AppendLine($"Old Inv PnL: {oldInventoryPnL:+0.00;-0.00;0.00}$");

            return messageText.ToString();
        }
    }
}
