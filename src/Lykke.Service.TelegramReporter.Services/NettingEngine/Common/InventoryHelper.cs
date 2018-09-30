using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.MarketMakerReports.Client.Models.InventorySnapshots;
using Lykke.Service.TelegramReporter.Core.Consts;
using Lykke.Service.TelegramReporter.Services.Extensions;

namespace Lykke.Service.TelegramReporter.Services.NettingEngine.Common
{
    public static class InventoryHelper
    {
        public static async Task<InventoryReportViewModel> GetAssetInventory(InventorySnapshotModel snapshot,
            IAssetsServiceWithCache assetsServiceWithCache)
        {
            var inventoryExchanges = snapshot?.Assets?.SelectManySafe(
                asset => asset.Inventories?.Select(o => o.Exchange));
            var balanceExchanges = snapshot?.Assets?.SelectManySafe(
                asset => asset.Balances?.Select(o => o.Exchange));

            IReadOnlyList<string> exchanges = Enumerable.Union(
                    inventoryExchanges ?? Enumerable.Empty<string>(),
                    balanceExchanges ?? Enumerable.Empty<string>())
                .Distinct().ToArray();

            if (exchanges.Contains(ExchangeNames.Lykke))
                exchanges = new[] { ExchangeNames.Lykke }.Union(exchanges.OrderBy(o => o)).Distinct().ToList();

            IReadOnlyList<string> inventoryAssets = (snapshot?.Assets?.Select(o => o.AssetId).Distinct() ?? Enumerable.Empty<string>())
                .ToArray();

            var rows = new List<RowViewModel>();

            var summary = new Dictionary<string, List<ExchangeSummaryViewModel>>();

            foreach (string assetId in inventoryAssets)
            {
                Asset asset = await assetsServiceWithCache.TryGetAssetAsync(assetId);

                var exchangeSummary = new List<ExchangeSummaryViewModel>();

                foreach (string exchange in exchanges)
                {
                    AssetBalanceInventoryModel assetBalance = snapshot?.Assets?.FirstOrDefault(o => o.AssetId == assetId);

                    AssetBalanceModel balance = assetBalance?.Balances?.FirstOrDefault(o => o.Exchange == exchange);

                    AssetInventoryModel inventory = assetBalance?.Inventories?.FirstOrDefault(o => o.Exchange == exchange);

                    var assetExchangeSummary = new ExchangeSummaryViewModel
                    {
                        Balance = balance?.Amount ?? 0,
                        BalanceUsd = balance?.AmountInUsd ?? 0,
                        BalanceRatio = 0,
                        Inventory = inventory?.Volume ?? 0,
                        InventoryUsd = inventory?.VolumeInUsd ?? 0,
                        Sell = inventory?.SellVolume ?? 0,
                        Buy = inventory?.BuyVolume ?? 0
                    };

                    if (!summary.ContainsKey(exchange))
                        summary[exchange] = new List<ExchangeSummaryViewModel>();

                    summary[exchange].Add(assetExchangeSummary);
                    exchangeSummary.Add(assetExchangeSummary);
                }

                rows.Add(new RowViewModel
                {
                    Asset = new ItemViewModel(assetId, asset?.DisplayId ?? assetId),
                    Exchanges = exchangeSummary,
                    Summary = new ExchangeSummaryViewModel
                    {
                        Balance = exchangeSummary.Sum(o => o.Balance),
                        BalanceUsd = exchangeSummary.Sum(o => o.BalanceUsd),
                        Inventory = exchangeSummary.Sum(o => o.Inventory),
                        InventoryUsd = exchangeSummary.Sum(o => o.InventoryUsd),
                        Sell = exchangeSummary.Sum(o => o.Sell),
                        Buy = exchangeSummary.Sum(o => o.Buy)
                    }
                });
            }

            var total = new List<ExchangeTotalViewModel>();

            foreach (KeyValuePair<string, List<ExchangeSummaryViewModel>> pair in summary)
            {
                var exchangeTotal = new ExchangeTotalViewModel
                {
                    BalanceUsd = pair.Value.Sum(o => o.BalanceUsd),
                    InventoryUsd = pair.Value.Sum(o => o.InventoryUsd)
                };

                total.Add(exchangeTotal);

                if (exchangeTotal.BalanceUsd == 0)
                    continue;

                foreach (ExchangeSummaryViewModel exchangeSummary in pair.Value)
                    exchangeSummary.BalanceRatio = exchangeSummary.BalanceUsd / Math.Abs(exchangeTotal.BalanceUsd);
            }

            decimal totalBalanceUsd = Math.Abs(total.Sum(o => o.BalanceUsd));

            if (totalBalanceUsd > 0)
            {
                foreach (RowViewModel row in rows)
                    row.Summary.BalanceRatio = row.Summary.BalanceUsd / totalBalanceUsd;
            }

            var vm = new InventoryReportViewModel
            {
                Exchanges = exchanges,
                Rows = rows
                    .OrderBy(o => o.Asset.Title)
                    .ToList(),
                Total = new TotalRowViewModel
                {
                    Exchanges = total
                }
            };

            return vm;
        }
    }
}
